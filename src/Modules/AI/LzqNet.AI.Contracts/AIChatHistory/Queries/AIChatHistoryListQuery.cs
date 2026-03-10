using LzqNet.AI.Domain.Enums;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.AI.Contracts.AIChatHistory.Queries;

public record AIChatHistoryListQuery : Query<List<AIChatHistoryViewDto>>
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

    public override List<AIChatHistoryViewDto> Result { get; set; }
    public AIChatHistoryListQuery()
    {
    }
}