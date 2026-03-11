# LzqNet.Extensions.AI 开发指南

## 1. 快速配置

在 `appsettings.json` 中配置多模型服务，并在 `Program.cs` 中通过 `IHostApplicationBuilder` 一行代码集成。

### 配置文件

``` JSON
{
  "AISettings": [
    {
      "ConfigId": "DeepSeekChat",
      "Url": "https://api.deepseek.com/v1",
      "KeySecret": "sk-xxx",
      "Model": "deepseek-chat"
    },
    {
      "ConfigId": "DeepSeekReasoner",
      "Url": "https://api.deepseek.com/v1",
      "KeySecret": "sk-xxx",
      "Model": "deepseek-reasoner"
    }
  ]
}
```

### 服务注册

``` C#
// 自动绑定 AISettings 节点并初始化服务
builder.AddLzqAI(); 
```

---

## 2. 智能体基础操作

通过 `IAIAgentService` 统一管理 `ChatClient` 与 `AIAgent` 的生命周期。

### 文本交互（同步/流式）
 
``` C#
// 1. 获取指定配置的客户端
var chatClient = _aiAgentService.GetChatClient("DeepSeekChat");

// 2. 创建智能体实例
var model = new AIAgentModel { Name = "知心伙伴" };
var agent = _aiAgentService.CreateAIAgent(chatClient, model);

// 3. 普通调用
var response = await _aiAgentService.RunAsync(agent, "你好");

// 4. 流式调用
await _aiAgentService.RunStreamingAsync(agent, "讲个故事", async (chunk) => 
{
    Console.Write(chunk); // 实时输出
});
```

---

## 3. 函数调用 (AITools)

支持通过 `AIFunctionFactory` 快速将 C# 方法扩展为 AI 的插件能力。

``` C#
var model = new AIAgentModel()
{
    ChatOptions = new ChatOptions()
    {
        Instructions = "你是一个天气助手",
        Tools = [AIFunctionFactory.Create(AIFunctionTools.GetWeather)]
    }
};

public class AIFunctionTools
{
    [Description("获取指定地点的天气")]
    public static string GetWeather([Description("地点名称")] string location) 
        => $"{location}今天晴，15°C。";
}
```

---

## 4. 记忆与持久化

通过 `AgentSession` 和 `ChatHistoryProvider` 实现跨请求的对话记忆。

### 多轮对话会话

``` C#
var agent = _aiAgentService.CreateAIAgent("DeepSeekChat", AIAgentConst.CHAT);
AgentSession session = await agent.CreateSessionAsync();

// 自动维护上下文记忆
await _aiAgentService.RunAsync(agent, "我的名字是 Tom", session);
var result = await _aiAgentService.RunAsync(agent, "我叫什么？", session); // 输出: 你叫 Tom
```

### 会话序列化与持久化

当需要将对话保存到数据库（如向量库或关系型数据库）并在未来恢复时：

``` C#
// 1. 序列化当前会话
JsonElement serialized = await agent.SerializeSessionAsync(session);
string json = serialized.ToString(); // 持久化此字符串

// 2. 恢复会话
JsonElement savedElement = JsonDocument.Parse(json).RootElement;
var restoredSession = await agent.DeserializeSessionAsync(savedElement);

// 3. 继续对话
var text = await _aiAgentService.RunAsync(agent, "还记得我吗？", restoredSession);
```

---

## 5. 任务工作流 (Workflow)

支持复杂的逻辑编排，将任务拆分为多个步骤（Step/Executor）。

### 线性工作流示例

``` C#
var stepOne = new StepOne();
var stepTwo = new StepTwo();

var workflow = new WorkflowBuilder(stepOne)
    .AddEdge(stepOne, stepTwo) // StepOne -> StepTwo
    .WithOutputFrom(stepTwo)
    .Build();

// 执行并监听过程事件
await using Run run = await InProcessExecution.RunAsync(workflow, "启动任务");
foreach (WorkflowEvent evt in run.NewEvents)
{
    if (evt is ExecutorCompletedEvent comp)
        Console.WriteLine($"步骤 {comp.ExecutorId} 完成: {comp.Data}");
}
```

### 步骤定义

``` C#
public sealed class StepOne() : Executor<string, string>("StepOne")
{
    public override async ValueTask<string> HandleAsync(string input, IWorkflowContext context, CancellationToken ct)
    {
        return "数据已处理";
    }
}
```

---

## 6. 技术架构要点

- **解耦设计**：`ChatClient` 负责通讯，`AIAgent` 负责逻辑，`Session` 负责状态。
- **自动依赖注入**：通过 `AddLzqAI` 自动处理 `IOptions` 绑定与作用域管理。
- **可扩展性**：支持自定义 `ChatHistoryProvider` 以接入不同的向量存储方案。