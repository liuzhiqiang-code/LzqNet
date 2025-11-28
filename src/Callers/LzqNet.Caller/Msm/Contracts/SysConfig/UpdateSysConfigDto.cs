using System.ComponentModel.DataAnnotations;

namespace LzqNet.Caller.Msm.Contracts.SysConfig;
public class UpdateSysConfigDto : AddSysConfigDto
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Required(ErrorMessage = "Id不能为空")]
    public required long Id { get; set; }
}
