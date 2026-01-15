namespace Masa.Contrib.Dispatcher.IntegrationEvents.RabbitMq;

public class RabbitMqOptions
{
    /// <summary>
    /// 主机地址
    /// </summary>
    public string HostName { get; set; }

    /// <summary>
    /// 端口号 (默认: 5672)
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// 用户名 (默认: admin)
    /// </summary>
    public string UserName { get; set; } = "admin";

    /// <summary>
    /// 密码 (默认: 123456)
    /// </summary>
    public string Password { get; set; } = "123456";

    /// <summary>
    /// 虚拟主机 (默认: /)
    /// </summary>
    public string VirtualHost { get; set; } = "/";
}
