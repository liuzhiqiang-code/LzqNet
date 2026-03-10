using LzqNet.AI.Domain.Enums;

namespace LzqNet.AI.Contracts.AIChatHistory;

public class AIChatHistoryViewDto
{
    /// <summary>
    /// Id
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// AIChatsId
    /// </summary>
    public long? AIChatsId { get; set; }

    /// <summary>
    /// Content
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// MessageType
    /// </summary>
    public MessageTypeEnum? MessageType { get; set; }
}
