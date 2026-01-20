namespace LzqNet.QueueConsumers.Notifications.CommandHandlers.Contracts;

public class DDResposeModel
{
    /// <summary>
    /// 返回代码
    /// </summary>
    public string errcode { get; set; }

    /// <summary>
    /// 返回状态
    /// </summary>
    public string errmsg { get; set; }

    /// <summary>
    /// 返回数据对象
    /// </summary>
    public object result { get; set; }

    /// <summary>
    /// 报告ID
    /// </summary>
    public string request_id { get; set; }
}