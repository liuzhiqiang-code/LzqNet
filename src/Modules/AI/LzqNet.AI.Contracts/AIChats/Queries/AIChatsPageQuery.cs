using LzqNet.Common.Contracts;

namespace LzqNet.AI.Contracts.AIChats.Queries;

public record AIChatsPageQuery : PageQuery<AIChatsViewDto>
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

    public override PageList<AIChatsViewDto> Result { get; set; }
    public AIChatsPageQuery()
    {
    }
}