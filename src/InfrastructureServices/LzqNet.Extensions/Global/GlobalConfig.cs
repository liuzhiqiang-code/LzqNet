namespace LzqNet.Extensions.Global;

public class GlobalConfig
{
    /// <summary>
    /// true用LzqNet.Auth授权，false使用单体jwt授权
    /// </summary>
    public bool UseAuth { get; set; } = false;
}
