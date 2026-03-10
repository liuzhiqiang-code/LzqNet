using LzqNet.Extensions.AI.Interfaces;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using OpenAI.Chat;

namespace LzqNet.Extensions.AI.Services
{
    /// <summary>
    /// AI服务
    /// </summary>
    public class AIAgentService(IChatClientService chatClientService, ChatHistoryProvider chatHistoryProvider) : IAIAgentService
    {
        private readonly IChatClientService _chatClientService = chatClientService;
        private readonly ChatHistoryProvider _chatHistoryProvider = chatHistoryProvider;
        public static string AIRulePrompt = "";

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <returns></returns>
        public IChatClient GetChatClient(string chatClientModel)
        {
            return _chatClientService.GetChatClient(chatClientModel);
        }

        public AIAgent CreateAIAgent(IChatClient chatClient, AIAgentModel aIAgentModel)
        {
            ArgumentNullException.ThrowIfNull(chatClient);
            ArgumentNullException.ThrowIfNull(aIAgentModel);

            return chatClient.AsAIAgent(new ChatClientAgentOptions()
            {
                Name = aIAgentModel.Name,
                ChatOptions = aIAgentModel.ChatOptions,
                Description = aIAgentModel.Description,
                ChatHistoryProvider = _chatHistoryProvider,
            });
        }

        public AIAgent CreateAIAgent(string chatClientModel, AIAgentModel aIAgentModel)
        {
            var chatClient = GetChatClient(chatClientModel);
            return CreateAIAgent(chatClient, aIAgentModel);
        }

        public async Task<string> RunAsync(AIAgent aiAgent, string message, AgentSession? agentSession = null)
        {
            var reslut = await aiAgent.RunAsync(message, agentSession);
            return reslut.Text;
        }

        public async Task<string> RunStreamingAsync(AIAgent aiAgent, string message, Func<string,Task> streameCallbackAsync)
        {
            var resultText = string.Empty;
            await foreach (AgentResponseUpdate update in aiAgent.RunStreamingAsync(message))
            {
                if (!string.IsNullOrEmpty(update.Text))
                {
                    await streameCallbackAsync.Invoke(update.Text);
                    resultText += update.Text;
                }
            }
            return resultText;
        }

        public async Task<(AIAgent, string)> CreateAIAgentAndRunAsync(string chatClientModel, AIAgentModel aIAgentModel, string message)
        {
            var chatClient = GetChatClient(chatClientModel);
            var aiAgent = CreateAIAgent(chatClient, aIAgentModel);
            var reslut = await RunAsync(aiAgent, message);
            return (aiAgent, reslut);
        }

        public async Task<(AIAgent, string)> CreateAIAgentAndRunStreamingAsync(string chatClientModel, AIAgentModel aIAgentModel, string message, Func<string,Task> streameCallbackAsync)
        {
            var chatClient = GetChatClient(chatClientModel);
            var aiAgent = CreateAIAgent(chatClient, aIAgentModel);
            var resultText = await RunStreamingAsync(aiAgent, message, streameCallbackAsync);
            return (aiAgent, resultText);
        }


        /// <summary>
        /// 智能体转换为McpServerTool
        /// </summary>
        /// <param name="aIAgent">智能体</param>
        /// <returns></returns>
        public McpServerTool AIAgentAsMcpServerTool(AIAgent aIAgent)
        {
            return McpServerTool.Create(aIAgent.AsAIFunction());
        }

        public Task<string> RunStreamingAsync(AIAgent aiAgent, string message, Action<string> streameCallback)
        {
            throw new NotImplementedException();
        }
    }
}
