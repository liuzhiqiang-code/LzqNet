using System.ComponentModel.DataAnnotations;

namespace LzqNet.Contracts.Msm.SysConfig;
public class UpdateSysConfigDto : AddSysConfigDto
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Required(ErrorMessage = "Id不能为空")]
    public required long Id { get; set; }
}
