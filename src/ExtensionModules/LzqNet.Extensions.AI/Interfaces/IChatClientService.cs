using LzqNet.Extensions.AI;
using Microsoft.Extensions.AI;

namespace LzqNet.Extensions.AI.Interfaces;

/// <summary>
/// AI聊天客户端服务接口
/// </summary>
public interface IChatClientService
{
    /// <summary>
    /// 获取指定配置的聊天客户端
    /// </summary>
    /// <param name="configId">配置ID</param>
    /// <returns>聊天客户端实例</returns>
    IChatClient GetChatClient(string configId);

    /// <summary>
    /// 获取新的聊天客户端
    /// </summary>
    /// <param name="configId">配置ID</param>
    /// <returns>聊天客户端实例</returns>
    IChatClient GetChatClient(AISetting setting);

}