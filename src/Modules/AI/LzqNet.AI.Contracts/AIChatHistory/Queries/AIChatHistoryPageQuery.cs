using LzqNet.AI.Domain.Enums;
using LzqNet.Common.Contracts;

namespace LzqNet.AI.Contracts.AIChatHistory.Queries;

public record AIChatHistoryPageQuery : PageQuery<AIChatHistoryViewDto>
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

    public override PageList<AIChatHistoryViewDto> Result { get; set; }

    public AIChatHistoryPageQuery()
    {
    }
}