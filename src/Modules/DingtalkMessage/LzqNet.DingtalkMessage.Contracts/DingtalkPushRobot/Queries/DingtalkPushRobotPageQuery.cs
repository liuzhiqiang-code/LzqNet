using LzqNet.DingtalkMessage.Contracts.DingtalkPushRobot;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.DingtalkMessage.Contracts.DingtalkPushRobot.Queries;

public record DingtalkPushRobotPageQuery : Query<PaginatedListBase<DingtalkPushRobotViewDto>>
{
    public DingtalkPushRobotPageSearchDto SearchDto { get; set; }
    public override PaginatedListBase<DingtalkPushRobotViewDto> Result { get; set; }
    public DingtalkPushRobotPageQuery(DingtalkPushRobotPageSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}