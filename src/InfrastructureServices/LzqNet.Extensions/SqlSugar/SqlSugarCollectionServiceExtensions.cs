using LzqNet.Extensions.SqlSugar.Config;
using LzqNet.Extensions.SqlSugar.Repository;
using SqlSugar;

namespace LzqNet.Extensions.SqlSugar;

public static class SqlSugarCollectionServiceExtensions
{
    public static WebApplicationBuilder AddCustomSqlSugar(this WebApplicationBuilder builder)
    {
        var dBConfigs = builder.Configuration.GetSection("DBConfigs").Get<List<DBConfig>>()
            ?? throw new MasaArgumentException("没有配置DBConfigs");
        var connectionConfigs = new List<ConnectionConfig>();
        foreach (var item in dBConfigs)
        {
            connectionConfigs.Add(new ConnectionConfig
            {
                ConfigId = item.Tag,
                DbType = item.DbType,
                ConnectionString = item.ConnectionString,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = ConfigureExternalServices,
            });
        }
        SqlSugarScope sqlSugar = new SqlSugarScope(connectionConfigs, db =>
        {
            foreach (var item in connectionConfigs)
            {
                new SqlSugarAopService().Init(db.GetConnection(item.ConfigId));
            }
        });

        SqlSugarHelper.Init(sqlSugar);
        builder.Services.AddTransient(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));

        return builder;
    }

    private static ConfigureExternalServices ConfigureExternalServices =>
         new ConfigureExternalServices()
         {
             //注意:  这儿AOP设置不能少   Nullable类型自动数据库变可空类型
             EntityService = (type, column) =>
             {
                 // int?  decimal?这种 isnullable=true
                 if (type.PropertyType.IsGenericType &&
                    type.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                 {
                     column.IsNullable = true;
                 }
                 column.DbColumnName = column.DbColumnName?.ToLower();//转小写
             },
             EntityNameService = (type, entity) =>
             {
                 entity.DbTableName = entity.DbTableName?.ToLower();//转小写
             },
         };
}