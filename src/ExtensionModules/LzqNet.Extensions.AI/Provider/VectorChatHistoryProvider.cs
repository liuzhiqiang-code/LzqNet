using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System.Text.Json;

namespace LzqNet.AI.Provider;

/// <summary>
/// A sample implementation of <see cref="ChatHistoryProvider"/> that stores chat history in a vector store.
/// State (the session DB key) is stored in the <see cref="AgentSession.StateBag"/> so it roundtrips
/// automatically with session serialization.
/// </summary>
internal sealed class VectorChatHistoryProvider : ChatHistoryProvider
{
    private readonly ProviderSessionState<State> _sessionState;
    private IReadOnlyList<string>? _stateKeys;
    private readonly VectorStore _vectorStore;

    public VectorChatHistoryProvider(
        VectorStore vectorStore,
        Func<AgentSession?, State>? stateInitializer = null,
        string? stateKey = null)
    {
        this._sessionState = new ProviderSessionState<State>(
            stateInitializer ?? (_ => new State(Guid.NewGuid().ToString("N"))),
            stateKey ?? this.GetType().Name);
        this._vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
    }

    public override IReadOnlyList<string> StateKeys => this._stateKeys ??= [this._sessionState.StateKey];

    public string GetSessionDbKey(AgentSession session)
        => this._sessionState.GetOrInitializeState(session).SessionDbKey;

    protected override async ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(InvokingContext context, CancellationToken cancellationToken = default)
    {
        var state = this._sessionState.GetOrInitializeState(context.Session);
        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);

        var records = await collection
            .GetAsync(
                x => x.SessionId == state.SessionDbKey, 10,
                new() { OrderBy = x => x.Descending(y => y.Timestamp) },
                cancellationToken)
            .ToListAsync(cancellationToken);

        var messages = records.ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!);
        messages.Reverse();
        return messages;
    }

    protected override async ValueTask StoreChatHistoryAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        var state = this._sessionState.GetOrInitializeState(context.Session);

        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);

        var allNewMessages = context.RequestMessages.Concat(context.ResponseMessages ?? []);

        await collection.UpsertAsync(allNewMessages.Select(x => new ChatHistoryItem()
        {
            Key = state.SessionDbKey + x.MessageId,
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = state.SessionDbKey,
            SerializedMessage = JsonSerializer.Serialize(x),
            MessageText = x.Text
        }), cancellationToken);
    }

    /// <summary>
    /// Represents the per-session state stored in the <see cref="AgentSession.StateBag"/>.
    /// </summary>
    public sealed class State
    {
        public State(string sessionDbKey)
        {
            this.SessionDbKey = sessionDbKey ?? throw new ArgumentNullException(nameof(sessionDbKey));
        }

        public string SessionDbKey { get; }
    }

    /// <summary>
    /// The data structure used to store chat history items in the vector store.
    /// </summary>
    private sealed class ChatHistoryItem
    {
        [VectorStoreKey]
        public string? Key { get; set; }

        [VectorStoreData]
        public string? SessionId { get; set; }

        [VectorStoreData]
        public DateTimeOffset? Timestamp { get; set; }

        [VectorStoreData]
        public string? SerializedMessage { get; set; }

        [VectorStoreData]
        public string? MessageText { get; set; }
    }
}