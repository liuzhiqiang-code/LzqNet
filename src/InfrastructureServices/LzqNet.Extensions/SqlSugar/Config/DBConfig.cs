using DbType = SqlSugar.DbType;

namespace LzqNet.Extensions.SqlSugar.Config;

/// <summary>
/// 数据库配置
/// </summary>
public class DBConfig
{
    /// <summary>
    /// 数据库标识
    /// </summary>
    public string Tag { get; set; }

    /// <summary>
    /// 数据库类型
    ///  MySql = 0, SqlServer = 1,Sqlite = 2, Oracle = 3, PostgreSQL = 4, Dm = 5,Kdbndp = 6,Oscar = 7,MySqlConnector = 8,
    ///  Access = 9,OpenGauss = 10,QuestDB = 11,HG = 12,ClickHouse = 13, GBase = 14, Odbc = 0xF, Custom = 900
    /// </summary>
    public DbType DbType { get; set; }

    /// <summary>
    /// 数据库连接串
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// 数据库超时时间
    /// </summary>
    public int CommandTimeOut { get; set; }
}