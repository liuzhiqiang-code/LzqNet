using LzqNet.Extensions.SqlSugar.Entities;
using SqlSugar;

namespace LzqNet.AI.Domain.Entities;

[Tenant("MsmConnection"), SugarTable("msm_ai_chats")]
public class AIChatsEntity : BaseFullEntity
{
    [SugarColumn(ColumnName = "ai_chats_name", Length = 100)]
    public string? AIChatsName { get; set; }

    [SugarColumn(ColumnName = "last_message", Length = 500)]
    public string? LastMessage { get; set; }

    [SugarColumn(ColumnName = "user_id")]
    public long UserId { get; set; }
}
