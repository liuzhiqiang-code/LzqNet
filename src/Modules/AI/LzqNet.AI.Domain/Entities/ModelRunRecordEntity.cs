using LzqNet.Extensions.SqlSugar.Entities;
using SqlSugar;

namespace LzqNet.AI.Domain.Entities;

[Tenant("MsmConnection"), SugarTable("msm_model_run_record")]
public class ModelRunRecordEntity : BaseFullEntity
{
    [SugarColumn(ColumnName = "chat_client", Length = 100)]
    public string ChatClient { get; set; }

    [SugarColumn(ColumnName = "ai_agent_model", IsJson = true)]
    public AIAgentModel AIAgentModel { get; set; }

    [SugarColumn(ColumnName = "ai_agent_name", Length = 100)]
    public string AIAgentName { get; set; }

    [SugarColumn(ColumnName = "instructions", ColumnDataType = "longtext")]
    public string? Instructions { get; set; }

    [SugarColumn(ColumnName = "prompt", ColumnDataType = "longtext")]
    public string Prompt { get; set; }

    [SugarColumn(ColumnName = "content", ColumnDataType = "longtext")]
    public string Content { get; set; }

    [SugarColumn(ColumnName = "prompt_tokens")]
    public long PromptTokens { get; set; }

    [SugarColumn(ColumnName = "completion_tokens")]
    public long CompletionTokens { get; set; }

    [SugarColumn(ColumnName = "duration_ms")]
    public long DurationMs { get; set; }

    [SugarColumn(ColumnName = "is_success")]
    public bool IsSuccess { get; set; }

    [SugarColumn(ColumnName = "error_message")]
    public string? ErrorMessage { get; set; }
}
