using System.Data;

namespace LzqNet.Common.Attributes;

/// <summary>
/// 事务单元 - 标记命令是否需要事务支持
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UnitOfWorkAttribute : Attribute
{
    /// <summary>
    /// 事务隔离级别
    /// </summary>
    public IsolationLevel IsolationLevel { get; set; }

    public UnitOfWorkAttribute(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        IsolationLevel = isolationLevel;
    }
}