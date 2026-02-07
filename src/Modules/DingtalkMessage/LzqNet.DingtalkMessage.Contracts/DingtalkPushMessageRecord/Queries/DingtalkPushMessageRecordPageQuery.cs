using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.DingtalkMessage.Contracts.DingtalkPushMessageRecord.Queries;

public record DingtalkPushMessageRecordPageQuery : Query<PaginatedListBase<DingtalkPushMessageRecordViewDto>>
{
    public DingtalkPushMessageRecordPageSearchDto SearchDto { get; set; }
    public override PaginatedListBase<DingtalkPushMessageRecordViewDto> Result { get; set; }
    public DingtalkPushMessageRecordPageQuery(DingtalkPushMessageRecordPageSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}