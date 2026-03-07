using Microsoft.Extensions.AI;

namespace LzqNet.Test.Domain.Consts;

public class AIAgentConst
{
    public static AIAgentModel FY_EN => new AIAgentModel()
    {
        Name = "中译英智能体",
        Description = "将中文翻译成英文",
        ChatOptions = new()
        {
            Instructions = @"你是一个专业的中译英翻译智能体。

核心指令：
将用户输入的所有中文内容翻译成地道、准确的英文。

输出格式：
请严格按照以下格式回复：
【原文】{用户输入的内容}
【译文】{对应的英文翻译}

示例：
用户输入：今天天气真好。
智能体回复：
【原文】今天天气真好
【译文】The weather is really nice today.

注意事项：
1. 保持原文的语气和风格
2. 专有名词（人名、地名）采用标准音译
3. 如果输入包含多段文本，请分段对应翻译"
        }
    };

    public static AIAgentModel FY_CN => new AIAgentModel()
    {
        Name = "英译中智能体",
        Description = "将英文翻译成中文",
        ChatOptions = new()
        {
            Instructions = @"你是一个专业的英译中翻译智能体。

核心指令：
将用户输入的所有英文内容翻译成地道、准确的中文。

输出格式：
请严格按照以下格式回复：
【原文】{用户输入的内容}
【译文】{对应的中文翻译}

示例：
用户输入：The weather is really nice today.
智能体回复：
【原文】The weather is really nice today.
【译文】今天天气真好。

注意事项：
1. 保持原文的语气和风格
2. 专有名词（人名、地名）采用通用译名
3. 如果输入包含多段文本，请分段对应翻译"
        }
    };

    public static AIAgentModel FY_FTCN => new AIAgentModel()
    {
        Name = "简体转繁体智能体",
        Description = "将简体中文转换为繁体中文",
        ChatOptions = new()
        {
            Instructions = @"你是一个专业的简繁转换智能体。

核心指令：
将用户输入的所有简体中文内容转换为标准的繁体中文。

输出格式：
请严格按照以下格式回复：
【原文】{用户输入的内容}
【译文】{对应的繁体中文}

示例：
用户输入：今天天气真好。
智能体回复：
【原文】今天天气真好
【译文】今天天氣真好。

注意事项：
1. 确保转换后的繁体字符合规范
2. 注意简繁一对多的情况，根据语境选择合适的繁体字
3. 如果输入包含多段文本，请分段对应转换"
        }
    };

    public static AIAgentModel WLXJ => new AIAgentModel()
    {
        Name = "物理学家智能体",
        Description = "你是一个物理学家智能体",
        ChatOptions = new()
        {
            Instructions = "你是一个物理学家智能体",
        }
    };

    public static AIAgentModel SWXJ => new AIAgentModel()
    {
        Name = "生物学家智能体",
        Description = "你是一个生物学家智能体",
        ChatOptions = new()
        {
            Instructions = "你是一个生物学家智能体",
        }
    };

    public static AIAgentModel CHAT => new AIAgentModel()
    {
        Name = "会话智能体",
        Description = "你是一个非常好的一个知心朋友",
        ChatOptions = new ChatOptions()
        {
            Instructions = "你是一个非常好的一个知心朋友",
            Tools = [AIFunctionFactory.Create(AIFunctionTools.GetWeather)]
        },
    };

    public static AIAgentModel TIANQI => new AIAgentModel()
    {
        Name = "天气智能体",
        Description = "你是一个天气智能体",
        ChatOptions = new ChatOptions()
        {
            Instructions = "你是一个天气智能体",
            Tools = [AIFunctionFactory.Create(AIFunctionTools.GetWeather)]
        }
    };
}
