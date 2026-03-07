using LzqNet.Extensions.SqlSugar.Entities;
using SqlSugar;

namespace LzqNet.System.Domain.Entities;

[Tenant("MsmConnection"), SugarTable("sys_config")]
public class SysConfigEntity : BaseFullEntity
{
}
