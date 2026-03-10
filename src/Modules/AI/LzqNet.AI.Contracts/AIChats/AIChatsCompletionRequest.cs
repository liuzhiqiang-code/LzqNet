namespace LzqNet.AI.Contracts.AIChats;

public class AIChatsCompletionRequest
{
    public long? AIChatsId { get; set; }

    public string ChatClient { get; set; }

    public string Prompt { get; set; }
}
