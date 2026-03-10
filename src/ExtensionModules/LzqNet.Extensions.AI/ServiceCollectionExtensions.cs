using LzqNet.Extensions.AI.Interfaces;
using LzqNet.Extensions.AI.Provider;
using LzqNet.Extensions.AI.Services;
using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace LzqNet.Extensions.AI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAIAgentClient(this IHostApplicationBuilder builder)
        {
            builder.Services.AddOptions<List<AISetting>>().BindConfiguration("AISettings");

            // 注入消息持久化
            builder.Services.AddSingleton<VectorStore>(new InMemoryVectorStore());
            builder.Services.AddSingleton<ChatHistoryProvider, VectorChatHistoryProvider>();
            builder.Services.AddSingleton<IChatClientService, ChatClientService>();
            builder.Services.AddTransient<IAIAgentService, AIAgentService>();
        }
    }
}
