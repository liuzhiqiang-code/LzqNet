using SqlSugar;

namespace LzqNet.Extensions.SqlSugar.Attributes;

/// <summary>
/// 公用分页查询过滤特性
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class QueryFilterAttribute : Attribute
{
    public ConditionalType ConditionalType { get; }

    public QueryFilterAttribute(ConditionalType conditionalType)
    {
        ConditionalType = conditionalType;
    }
}