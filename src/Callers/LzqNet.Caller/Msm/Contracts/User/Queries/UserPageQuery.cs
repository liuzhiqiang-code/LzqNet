using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.Caller.Msm.Contracts.User.Queries;

public record UserPageQuery : Query<PaginatedListBase<UserViewDto>>
{
    public UserPageSearchDto SearchDto { get; set; }
    public override PaginatedListBase<UserViewDto> Result { get; set; }
    public UserPageQuery(UserPageSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}