using Yarp.ReverseProxy.Configuration;

namespace LzqNet.ApiGateway;

public class HotReloadProxyConfigProvider : IHotReloadProxyConfigProvider
{
    private volatile IProxyConfig _currentConfig;
    private readonly ReaderWriterLockSlim _configLock = new();

    /// <summary>
    /// 手动更新代理配置（线程安全）
    /// </summary>
    public void UpdateConfig(IProxyConfig newConfig)
    {
        _configLock.EnterWriteLock();
        try
        {
            _currentConfig = newConfig ?? throw new ArgumentNullException(nameof(newConfig));
        }
        finally
        {
            _configLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// 获取当前代理配置（线程安全）
    /// </summary>
    public IProxyConfig GetConfig()
    {
        _configLock.EnterReadLock();
        try
        {
            return _currentConfig;
        }
        finally
        {
            _configLock.ExitReadLock();
        }
    }

    public void Dispose()
    {
        _configLock.Dispose();
        GC.SuppressFinalize(this);
    }
}
