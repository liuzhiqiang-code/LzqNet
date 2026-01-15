using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushRobot.Queries;

public record DingtalkPushRobotPageQuery : Query<PaginatedListBase<DingtalkPushRobotViewDto>>
{
    public DingtalkPushRobotPageSearchDto SearchDto { get; set; }
    public override PaginatedListBase<DingtalkPushRobotViewDto> Result { get; set; }
    public DingtalkPushRobotPageQuery(DingtalkPushRobotPageSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}