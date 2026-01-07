using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.Caller.Msm.Contracts.User.Queries;

public record UserGetListQuery : Query<List<UserViewDto>>
{
    public UserSearchDto SearchDto { get; set; }
    public override List<UserViewDto> Result { get; set; }
    public UserGetListQuery(UserSearchDto? searchDto)
    {
        SearchDto = searchDto ?? new();
    }
}