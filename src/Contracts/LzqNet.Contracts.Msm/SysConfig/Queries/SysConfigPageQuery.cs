using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Contracts.Msm.SysConfig.Queries;

public record SysConfigPageQuery : Query<PaginatedListBase<SysConfigViewDto>>
{
    public SysConfigSearchDto SearchDto { get; set; }
    public override PaginatedListBase<SysConfigViewDto> Result { get; set; }
    public SysConfigPageQuery(SysConfigSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}
