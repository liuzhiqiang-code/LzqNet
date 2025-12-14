
namespace LzqNet.Caller.Msm.Contracts.Menu;
public class MenuViewDto
{
    public long Id { get; set; }
    public long? Pid { get; set; }
    public string AuthCode { get; set; }
    public string? Component { get; set; }
    public MenuMeta? Meta { get; set; }
    public string Name { get; set; }
    public EnableStatusEnum Status { get; set; }
    public string? Path { get; set; }
    public string? Redirect { get; set; }
    public string Type { get; set; }
    public List<MenuViewDto>? Children { get; set; }
}
