using SqlSugar;

namespace LzqNet.Extensions.SqlSugar;

public class SqlSugarHelper
{
    private static readonly object _lock = new object();
    private static SqlSugarScope _client;

    /// <summary>
    /// 获取SqlSugar单例实例
    /// </summary>
    public static SqlSugarScope Client
    {
        get
        {
            if (_client == null)
            {
                throw new InvalidOperationException("SqlSugar未初始化，请先调用Init方法");
            }
            return _client;
        }
    }

    /// <summary>
    /// 初始化SqlSugar
    /// </summary>
    public static void Init(SqlSugarScope sqlSugarClient)
    {
        lock (_lock)
        {
            if (_client != null)
            {
                throw new InvalidOperationException("SqlSugar已经初始化，请勿重复初始化");
            }
            _client = sqlSugarClient ?? throw new ArgumentNullException(nameof(sqlSugarClient));
        }
    }
}
