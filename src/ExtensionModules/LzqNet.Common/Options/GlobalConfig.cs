namespace LzqNet.Common.Options;

public class GlobalConfig
{
    /// <summary>
    /// true用LzqNet.Auth授权，false使用单体jwt授权
    /// </summary>
    public bool UseAuth { get; set; } = false;

    /// <summary>
    /// 使用swagger文档，默认不开启，使用网关集中swagger，单体启动可改为需要swagger
    /// </summary>
    public bool UseSwagger { get; set; } = false;
}
