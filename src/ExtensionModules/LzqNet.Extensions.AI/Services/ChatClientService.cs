using LzqNet.Extensions.AI.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using System.ClientModel;
using System.Collections.Concurrent;

namespace LzqNet.Extensions.AI.Services;

public class ChatClientService: IChatClientService
{
    private readonly ConcurrentDictionary<string, IChatClient> _chatClientDictionary = new();
    private readonly List<AISetting> _aiSettings;
    private readonly IOptionsMonitor<List<AISetting>> _optionsMonitor;

    public ChatClientService(IOptionsMonitor<List<AISetting>> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
        _aiSettings = optionsMonitor.CurrentValue;

        // 监听配置变化，当配置更新时清空字典（可选）
        _optionsMonitor.OnChange(newSettings =>
        {
            _chatClientDictionary.Clear();
        });
    }

    public IChatClient GetChatClient(string configId)
    {
        if (string.IsNullOrWhiteSpace(configId))
            throw new ArgumentException("ConfigId不能为空", nameof(configId));

        var aiSetting = _aiSettings.FirstOrDefault(x => x.ConfigId == configId);
        if (aiSetting == null)
            throw new InvalidOperationException($"未找到ConfigId为 '{configId}' 的配置项");

        // 使用GetOrAdd确保线程安全
        return _chatClientDictionary.GetOrAdd(configId, CreateChatClient(aiSetting));
    }

    public IChatClient GetChatClient(AISetting aiSetting)
    {
        if (aiSetting == null)
            throw new InvalidOperationException($"参数不能为空");

        return CreateChatClient(aiSetting);
    }

    private IChatClient CreateChatClient(AISetting aiSetting)
    {
        if (string.IsNullOrWhiteSpace(aiSetting.Url))
            throw new InvalidOperationException($"ConfigId '{aiSetting.ConfigId}' 的Url配置不能为空");

        if (string.IsNullOrWhiteSpace(aiSetting.KeySecret))
            throw new InvalidOperationException($"ConfigId '{aiSetting.ConfigId}' 的KeySecret配置不能为空");

        if (string.IsNullOrWhiteSpace(aiSetting.Model))
            throw new InvalidOperationException($"ConfigId '{aiSetting.ConfigId}' 的Model配置不能为空");

        try
        {
            var openAIClientOptions = new OpenAIClientOptions
            {
                Endpoint = new Uri(aiSetting.Url)
            };

            var openAIClient = new OpenAIClient(
                new ApiKeyCredential(aiSetting.KeySecret),
                openAIClientOptions);

            var chatClient = openAIClient.GetChatClient(aiSetting.Model);
            return chatClient.AsIChatClient();
        }
        catch (UriFormatException ex)
        {
            throw new InvalidOperationException($"ConfigId '{aiSetting.ConfigId}' 的Url格式无效: {aiSetting.Url}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"创建ChatClient时发生错误 (ConfigId: {aiSetting.ConfigId})", ex);
        }
    }
}
