namespace LzqNet.QueueConsumers.Notifications.CommandHandlers.Contracts;

public class DDGroupMsgRequest
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public string msgtype { get; set; }
    /// <summary>
    /// 消息内容
    /// </summary>
    public DDMsgContent markdown { get; set; }

    public AtUserInfo at { get; set; }
}
public class DDMsgContent
{
    /// <summary>
    /// 消息标题
    /// </summary>
    public string title { get; set; }
    /// <summary>
    /// 消息内容
    /// </summary>
    public string text { get; set; }
}
//@群成员
public class AtUserInfo
{
    public List<string> atMobiles { get; set; }
    public List<string> atUserIds { get; set; }
    public bool isAtAll { get; set; }
}