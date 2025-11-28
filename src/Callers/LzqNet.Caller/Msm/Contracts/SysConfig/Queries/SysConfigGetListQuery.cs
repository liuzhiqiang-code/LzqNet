using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Caller.Msm.Contracts.SysConfig.Queries;

public record SysConfigGetListQuery : Query<List<SysConfigViewDto>>
{
    public SysConfigSearchDto SearchDto { get; set; }
    public override List<SysConfigViewDto> Result { get; set; }
    public SysConfigGetListQuery(SysConfigSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}
