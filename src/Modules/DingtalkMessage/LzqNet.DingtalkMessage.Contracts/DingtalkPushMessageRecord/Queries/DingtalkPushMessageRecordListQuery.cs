using LzqNet.DingtalkMessage.Contracts.DingtalkPushMessageRecord;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.DingtalkMessage.Contracts.DingtalkPushMessageRecord.Queries;

public record DingtalkPushMessageRecordListQuery : Query<List<DingtalkPushMessageRecordViewDto>>
{
    public DingtalkPushMessageRecordSearchDto SearchDto { get; set; }
    public override List<DingtalkPushMessageRecordViewDto> Result { get; set; }
    public DingtalkPushMessageRecordListQuery(DingtalkPushMessageRecordSearchDto? searchDto)
    {
        SearchDto = searchDto ?? new();
    }
}