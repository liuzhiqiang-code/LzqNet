using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Extensions.SqlSugar.Config;

public class SugarOption
{
    public string Tag => DBConfig.Tag;
    public DBConfig DBConfig { get; set; }
    public SugarAopOption SugarAopOption { get; set; }
    public Action<QueryFilterProvider> QueryFilter { get; set; }
    public Action<SqlSugarProvider> CustomerAction { get; set; }
}

public class SugarAopOption
{
    public Action<SqlSugarProvider, string, SugarParameter[]> OnLogExecuting { get; set; }
    public Action<SqlSugarProvider, string, SugarParameter[]> OnLogExecuted { get; set; }
    public Action<SqlSugarProvider, SqlSugarException> OnError { get; set; }
    public Action<SqlSugarProvider, object, DataFilterModel> DataExecuting { get; set; }
}