public class JwtOption
{
    public string Audience { get; set; }
    public string Authority { get; set; }
    public bool RequireHttpsMetadata { get; set; }


    /// <summary>
    /// 发起人
    /// </summary>
    public string Issuer { get; set; }
    /// <summary>
    /// token过期时间，单位分钟，7200s
    /// </summary>
    public int AccessExpiration { get; set; } = 7200;

    /// <summary>
    /// 
    /// </summary>
    public string Secret { get; set; } = "";
    public string RefreshSecret { get; set; } = "";
    /// <summary>
    /// token过期时间，单位分钟，7200s
    /// </summary>
    public int RefreshExpirationDays { get; set; } = 72000;
}
