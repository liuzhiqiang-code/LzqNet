using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushRobot.Queries;

public record DingtalkPushRobotListQuery : Query<List<DingtalkPushRobotViewDto>>
{
    public DingtalkPushRobotSearchDto SearchDto { get; set; }
    public override List<DingtalkPushRobotViewDto> Result { get; set; }
    public DingtalkPushRobotListQuery(DingtalkPushRobotSearchDto? searchDto)
    {
        SearchDto = searchDto ?? new();
    }
}