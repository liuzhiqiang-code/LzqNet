namespace LzqNet.Extensions.SignalR.Models
{
    public class SignalRCacheDto
    {
        /// <summary>
        /// 用户组列表
        /// </summary>
        public List<SignalRRedisItemDto> Items { get; set; } = new List<SignalRRedisItemDto>();
    }
    public class SignalRRedisItemDto
    {

        /// <summary>
        ///租户id
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 链接ids
        /// </summary>
        public List<IdentityConnectionDto> Connections { get; set; } = new List<IdentityConnectionDto>();
    }
    public class IdentityConnectionDto
    {
        public string IdentityId { get; set; } = "";
        public string ConnectionId { get; set; } = "";

    }

}
