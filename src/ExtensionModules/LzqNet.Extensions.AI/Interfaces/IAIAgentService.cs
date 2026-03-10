using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace LzqNet.Extensions.AI.Interfaces
{
    public interface IAIAgentService
    {
        IChatClient GetChatClient(string chatClientModel);

        AIAgent CreateAIAgent(IChatClient chatClient, AIAgentModel aIAgentModel);

        AIAgent CreateAIAgent(string chatClientModel, AIAgentModel aIAgentModel);

        Task<string> RunAsync(AIAgent aiAgent, string message, AgentSession? agentSession = null);

        Task<string> RunStreamingAsync(AIAgent aiAgent, string message, Func<string,Task> streameCallbackAsync);

        Task<(AIAgent, string)> CreateAIAgentAndRunAsync(string chatClientModel, AIAgentModel aIAgentModel, string message);

        Task<(AIAgent, string)> CreateAIAgentAndRunStreamingAsync(string chatClientModel, AIAgentModel aIAgentModel, string message, Func<string,Task> streameCallbackAsync);

        McpServerTool AIAgentAsMcpServerTool(AIAgent aIAgent);
    }
}
