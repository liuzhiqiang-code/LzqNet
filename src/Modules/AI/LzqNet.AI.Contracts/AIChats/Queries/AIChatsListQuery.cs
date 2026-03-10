using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.AI.Contracts.AIChats.Queries;

public record AIChatsListQuery : Query<List<AIChatsViewDto>>
{
    /// <summary>
    /// Id
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// AIChatsName
    /// </summary>
    public string? AIChatsName { get; set; }

    /// <summary>
    /// LastMessage
    /// </summary>
    public string? LastMessage { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public long? UserId { get; set; }

    public override List<AIChatsViewDto> Result { get; set; }
    public AIChatsListQuery()
    {
    }
}