using Consul;
using Google.Protobuf;
using LzqNet.AI.Interfaces;
using LzqNet.Test.Contracts.TestContent.Commands;
using LzqNet.Test.Domain.Consts;
using MapsterMapper;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using NSwag.Annotations;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace LzqNet.Test.Application.Services;

public class TestAIService : ServiceBase
{
    public TestAIService() : base("/api/v1/testAI") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();
    private IAIAgentService AIAgentService => GetRequiredService<IAIAgentService>();

    [OpenApiTag("testAI", Description = "对话补全")]
    [RoutePattern(pattern: "chat", true)]
    public async Task<AdminResult> ChatAsync([FromBody] AiChatCommand command)
    {
        try
        {
            // 创建一个代理
            var chatClient = AIAgentService.GetChatClient(ChatClientConst.DeepSeekChat);
            var agentA = AIAgentService.CreateAIAgent(chatClient, AIAgentConst.WLXJ);

            // 创建一个智能体B
            var agentB = AIAgentService.CreateAIAgent(ChatClientConst.DeepSeekChat, AIAgentConst.FY_EN);

            // 智能体发消息
            var textA = await AIAgentService.RunAsync(agentA, "你是什么智能体");

            // 创建智能体并发消息
            var (agentC, textC) = await AIAgentService.CreateAIAgentAndRunAsync(ChatClientConst.DeepSeekChat, AIAgentConst.WLXJ, "你是什么智能体");

            return AdminResult.Success();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [OpenApiTag("testAI", Description = "对话补全流式输出")]
    [RoutePattern(pattern: "chatStream", true)]
    public async Task<AdminResult> ChatStreamAsync([FromBody] TestContentCreateCommand command)
    {
        var agentB = AIAgentService.CreateAIAgent(ChatClientConst.DeepSeekChat, AIAgentConst.FY_EN);
        var textB = await AIAgentService.RunStreamingAsync(agentB, "你是什么智能体", (text) =>
        {
            Console.WriteLine(text);
        });

        var (agentD, textD) = await AIAgentService.CreateAIAgentAndRunStreamingAsync(ChatClientConst.DeepSeekChat, AIAgentConst.WLXJ, "你是什么智能体", (text) =>
        {
            Console.WriteLine(text);
        });

        return AdminResult.Success();
    }

    [OpenApiTag("testAI", Description = "对话补全工具调用")]
    [RoutePattern(pattern: "chatWithTool", true)]
    public async Task<AdminResult> chatWithToolAsync([FromBody] TestContentCreateCommand command)
    {
        var agent = AIAgentService.CreateAIAgent(ChatClientConst.DeepSeekChat, AIAgentConst.TIANQI);
        var text = await AIAgentService.RunAsync(agent, "今天湖北的天气怎么样");
        /* 输出：  
         * 我来帮您查询今天湖北的天气情况。
            根据查询结果，今天湖北的天气情况是：
            **天气状况**：多云  
            **最高温度**：15°C  
            今天湖北是多云天气，气温适中，最高温度15°C，是比较舒适的天气。建议您根据具体活动安排适当的衣物。
         */
        return AdminResult.Success();
    }

    [OpenApiTag("testAI", Description = "多轮次对话")]
    [RoutePattern(pattern: "chatWithAgentSession", true)]
    public async Task<AdminResult> chatWithAgentSessionAsync([FromBody] TestContentCreateCommand command)
    {
        var agent = AIAgentService.CreateAIAgent(ChatClientConst.DeepSeekChat, AIAgentConst.CHAT);
        AgentSession session = await agent.CreateSessionAsync();

        var text1 = await AIAgentService.RunAsync(agent, "我的名字是 Tom", session);
        var text2 = await AIAgentService.RunAsync(agent, "我的名字是什么来着?", session);
        /* 输出：  
            text1  :  你好 Tom！很高兴认识你！我是你的AI助手，有什么我可以帮助你的吗？无论是聊天、回答问题，还是帮你查询天气信息（我有天气查询工具），我都很乐意为你提供帮助！
            text2  :  你的名字是 Tom！你刚才告诉我了，我记得很清楚呢！😊
         */
        return AdminResult.Success();
    }

    [OpenApiTag("testAI", Description = "内存与持久性")]
    [RoutePattern(pattern: "chatWithMemory", true)]
    public async Task<AdminResult> chatWithMemoryAsync([FromBody] TestContentCreateCommand command)
    {
        // 第1次请求    
        var agentA = AIAgentService.CreateAIAgent(ChatClientConst.DeepSeekChat, AIAgentConst.CHAT);
        var agentB = AIAgentService.CreateAIAgent(ChatClientConst.DeepSeekChat, AIAgentConst.CHAT);
        AgentSession session = await agentA.CreateSessionAsync();
        var text1 = await AIAgentService.RunAsync(agentA, "我的名字是 Tom", session);
        var text2 = await AIAgentService.RunAsync(agentA, "我的名字是什么来着?", session);
        var text3 = await AIAgentService.RunAsync(agentB, "我的名字是什么来着?", session);

        // 持久化AgentSession 到db 
        JsonElement serializedSession = await agentA.SerializeSessionAsync(session);
        string json = serializedSession.ToString();

        // 第2次请求  新的 agent
        // 请求时反序列化
        JsonElement savedElement = JsonDocument.Parse(json).RootElement;
        var restoredSession = await agentA.DeserializeSessionAsync(savedElement);
        var text4 = await AIAgentService.RunAsync(agentA, "我的名字是什么来着?", restoredSession);
        var text5 = await AIAgentService.RunAsync(agentB, "我的名字是什么来着?", restoredSession);

        var text = new StringBuilder()
            .AppendLine($"text1  : {text1}")
            .AppendLine($"text2  : {text2}")
            .AppendLine($"text3  : {text3}")
            .AppendLine($"text4  : {text4}")
            .AppendLine($"text5  : {text5}")
            .ToString();
        /* 输出：  
         * 持久化AgentSession到DB就行，与AIAgent对象无关，保证AIAgent里的持久化ChatHistoryProvider对象是同一个就行
         * ChatHistoryProvider一般直接注入为
         * builder.Services.AddSingleton<VectorStore>(new InMemoryVectorStore());
            builder.Services.AddSingleton<ChatHistoryProvider, VectorChatHistoryProvider>();
         * 
text1  : 你好Tom！很高兴认识你！😊
我是你的AI助手，可以帮你处理各种问题。比如，如果你想知道某个地方的天气情况，我可以帮你查询。或者如果你有其他需要帮助的事情，也尽管告诉我！
有什么我可以为你做的吗？
text2  : 你的名字是Tom！😊 你刚才告诉过我你的名字，我记得很清楚呢！
有什么其他需要我帮助的吗，Tom？
text3  : 你的名字是Tom！😊 我刚才已经告诉过你了，看来你可能有点健忘呢，不过没关系，我会一直记得你的名字的！
有什么其他需要我帮助的吗，Tom？
text4  : 你的名字是Tom！😊 你刚才已经问过我两次了，看来你今天可能有点健忘或者是在测试我是否记得你的名字。不过别担心，我会一直记得你是Tom的！
有什么其他需要我帮助的吗，Tom？
text5  : 你的名字是Tom！😊 这已经是你第三次问我了！看来你今天可能特别想确认我是否记得你的名字，或者你是在跟我开玩笑呢！
不过没关系，无论你问多少次，我都会记得你是Tom的！有什么其他需要我帮助的吗，Tom？
         */
        return AdminResult.Success();
    }

    [OpenApiTag("testAI", Description = "简单工作流")]
    [RoutePattern(pattern: "chatEasyWorkflow", true)]
    public async Task<AdminResult> ChatEsayWorkflowAsync([FromBody] TestContentCreateCommand command)
    {
        var text = new StringBuilder();
        var stepOne = new StepOne();
        var stepTwo = new StepTwo();
        var workflow = new WorkflowBuilder(stepOne)
           .AddEdge(stepOne, stepTwo)
           .WithOutputFrom(stepTwo)
           .Build();
        await using Run run = await InProcessExecution.RunAsync(workflow, "你是什么智能体");
        foreach (WorkflowEvent evt in run.NewEvents)
        {
            if (evt is SuperStepStartedEvent superStepStartedEvent)
            {
                text.AppendLine($"同步 SuperStepStartedEvent : {superStepStartedEvent.StepNumber}");
            }
            else if (evt is ExecutorInvokedEvent executorInvokedEvent)
            {
                text.AppendLine($"同步 ExecutorInvokedEvent {executorInvokedEvent.ExecutorId}: 【{executorInvokedEvent.Data}】");
            }
            else if (evt is ExecutorFailedEvent executorFailed)
            {
                text.AppendLine($"同步 ExecutorFailedEvent {executorFailed.ExecutorId}: 【{executorFailed.Data}】");
            }
            else if (evt is WorkflowErrorEvent workflowErrorEvent)
            {
                text.AppendLine($"同步 WorkflowErrorEvent : 【{workflowErrorEvent.Data}】");
            }
            else if (evt is ExecutorCompletedEvent executorComplete)
            {
                text.AppendLine($"同步 ExecutorCompletedEvent {executorComplete.ExecutorId}: {executorComplete.Data}");
            }
            else if (evt is SuperStepCompletedEvent superStepCompletedEvent)
            {
                text.AppendLine($"同步 SuperStepCompletedEvent : {superStepCompletedEvent.StepNumber}");
            }
            else if (evt is WorkflowOutputEvent output)
            {
                text.AppendLine($"同步 WorkflowOutputEvent {output.ExecutorId}: {output.Data}");
            }
            else
            {
                text.AppendLine($"同步 evt {evt.GetType().FullName}: 【{evt.Data}】");
            }
        }

        // 流
        var text2 = new StringBuilder();
        var workflow2 = new WorkflowBuilder(stepOne)
           .AddEdge(stepOne, stepTwo)
           .Build();
        await using StreamingRun run2 = await InProcessExecution.RunStreamingAsync(workflow2, "你是什么智能体");
        await foreach (WorkflowEvent evt in run2.WatchStreamAsync().ConfigureAwait(false))
        {
            if (evt is SuperStepStartedEvent superStepStartedEvent)
            {
                text2.AppendLine($"同步 SuperStepStartedEvent : {superStepStartedEvent.StepNumber}");
            }
            else if (evt is ExecutorInvokedEvent executorInvokedEvent)
            {
                text2.AppendLine($"同步 ExecutorInvokedEvent {executorInvokedEvent.ExecutorId}: 【{executorInvokedEvent.Data}】");
            }
            else if (evt is ExecutorFailedEvent executorFailed)
            {
                text2.AppendLine($"同步 ExecutorFailedEvent {executorFailed.ExecutorId}: 【{executorFailed.Data}】");
            }
            else if (evt is WorkflowErrorEvent workflowErrorEvent)
            {
                text2.AppendLine($"同步 WorkflowErrorEvent : 【{workflowErrorEvent.Data}】");
            }
            else if (evt is ExecutorCompletedEvent executorComplete)
            {
                text2.AppendLine($"同步 ExecutorCompletedEvent {executorComplete.ExecutorId}: {executorComplete.Data}");
            }
            else if (evt is SuperStepCompletedEvent superStepCompletedEvent)
            {
                text2.AppendLine($"同步 SuperStepCompletedEvent : {superStepCompletedEvent.StepNumber}");
            }
            else if (evt is WorkflowOutputEvent output)
            {
                text2.AppendLine($"同步 WorkflowOutputEvent {output.ExecutorId}: {output.Data}");
            }
            else
            {
                text2.AppendLine($"同步 evt {evt.GetType().FullName}: 【{evt.Data}】");
            }
        }
        return AdminResult.Success();
    }

    [OpenApiTag("testAI", Description = "工作流")]
    [RoutePattern(pattern: "chatWorkflow", true)]
    public async Task<AdminResult> ChatWorkflowAsync([FromBody] TestContentCreateCommand command)
    {
        var textResult = new StringBuilder();
        var agentA = AIAgentService.CreateAIAgent(ChatClientConst.DeepSeekChat, AIAgentConst.FY_EN);
        var agentB = AIAgentService.CreateAIAgent(ChatClientConst.DeepSeekChat, AIAgentConst.FY_CN);
        var agentC = AIAgentService.CreateAIAgent(ChatClientConst.DeepSeekChat, AIAgentConst.FY_FTCN);

        var workflow = new WorkflowBuilder(agentA)
            .AddEdge(agentA, agentB)
            .AddEdge(agentB, agentC)
            .WithOutputFrom(agentC)
            .Build();

        // Execute the workflow
        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, new ChatMessage(ChatRole.User, "我是一个专业的AI工作流"));

        // Must send the turn token to trigger the agents.
        // The agents are wrapped as executors. When they receive messages,
        // they will cache the messages and only start processing when they receive a TurnToken.
        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
        await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
        {
            if (evt is SuperStepStartedEvent superStepStartedEvent)
            {
                textResult.AppendLine($"同步 SuperStepStartedEvent : {superStepStartedEvent.StepNumber}");
            }
            else if (evt is ExecutorInvokedEvent executorInvokedEvent)
            {
                textResult.AppendLine($"同步 ExecutorInvokedEvent {executorInvokedEvent.ExecutorId}: 【{executorInvokedEvent.Data}】");
            }
            else if (evt is ExecutorFailedEvent executorFailed)
            {
                textResult.AppendLine($"同步 ExecutorFailedEvent {executorFailed.ExecutorId}: 【{executorFailed.Data}】");
            }
            else if (evt is WorkflowErrorEvent workflowErrorEvent)
            {
                textResult.AppendLine($"同步 WorkflowErrorEvent : 【{workflowErrorEvent.Data}】");
            }
            else if (evt is ExecutorCompletedEvent executorComplete)
            {
                textResult.AppendLine($"同步 ExecutorCompletedEvent {executorComplete.ExecutorId}: {executorComplete.Data}");
            }
            else if (evt is SuperStepCompletedEvent superStepCompletedEvent)
            {
                textResult.AppendLine($"同步 SuperStepCompletedEvent : {superStepCompletedEvent.StepNumber}");
            }
            else if (evt is WorkflowOutputEvent output)
            {
                textResult.AppendLine($"同步 WorkflowOutputEvent {output.ExecutorId}: {output.Data}");
            }
            else
            {
                textResult.AppendLine($"同步 evt {evt.GetType().FullName}: 【{evt.Data}】");
            }
        }
        Console.WriteLine(textResult);
        return AdminResult.Success();
    }
}

/// <summary>
/// 第一个步骤执行器 - StepOne Executor
/// </summary>
public sealed class StepOne() :
    Executor<string,string>("StepOne")
{
    public override async ValueTask<string> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        message = "第1个步骤重新赋值";
        return message;
    }
}

/// <summary>
/// 第二个步骤执行器 - StepTwo Executor
/// </summary>
public sealed class StepTwo() :
    Executor<string,string>("StepTwo")
{
    public override async ValueTask<string> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        if (message == "第1个步骤重新赋值") {
            message = "收到第1个步骤重新赋值";
        }
        return message;
    }
}