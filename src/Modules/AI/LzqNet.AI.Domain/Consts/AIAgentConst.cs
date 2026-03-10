using Microsoft.Extensions.AI;

namespace LzqNet.AI.Domain.Consts;

public class AIAgentConst
{
    public static AIAgentModel CHAT => new AIAgentModel()
    {
        Name = "智能对话助手",
        Description = "智能对话助手",
        ChatOptions = new ChatOptions()
        {
            Instructions = ""
        },
    };

    public static AIAgentModel TITLE => new AIAgentModel()
    {
        Name = "智能标题生成助手",
        Description = "多语言智能对话标题生成器",

        ChatOptions = new ChatOptions()
        {
            Instructions = @"生成标题(1-30字)：提炼核心，不用标点，避免冗余词。
示例：""Python怎么安装库""→""Python库安装方法""；""推荐科幻电影""→""科幻电影推荐""
输入：{content}
标题：",
            Temperature = 0.3f,  // 温度控制:	控制随机性，越低越确定性 
            MaxOutputTokens = 30,  // 最大输出token数
            TopP = 0.9f, // 核采样，控制多样性
            FrequencyPenalty = 0.1f,  // 减少重复token
            PresencePenalty = 0.1f,  // 减少已出现token
            StopSequences = new List<string> { "\n", "\r", "   " }  // 停止生成的序列
        },
    };
}
