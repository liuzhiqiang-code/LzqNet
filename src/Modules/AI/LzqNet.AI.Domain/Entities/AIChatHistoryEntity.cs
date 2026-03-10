using LzqNet.AI.Domain.Enums;
using LzqNet.Extensions.SqlSugar.Entities;
using SqlSugar;

namespace LzqNet.AI.Domain.Entities;

[Tenant("MsmConnection"), SugarTable("msm_ai_chat_history")]
public class AIChatHistoryEntity : BaseFullEntity
{
    [SugarColumn(ColumnName = "ai_chats_id")]
    public long AIChatsId { get; set; }

    [SugarColumn(ColumnName = "content", Length = 2000)]
    public string Content { get; set; }

    [SugarColumn(ColumnName = "message_type")]
    public MessageTypeEnum MessageType { get; set; }

    [SugarColumn(ColumnName = "file_name", Length = 200)]
    public string? FileName { get; set; }
}
