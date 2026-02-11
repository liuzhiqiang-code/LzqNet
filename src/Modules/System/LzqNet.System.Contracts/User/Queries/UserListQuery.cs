using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Contracts.User.Queries;

public record UserListQuery : Query<List<UserViewDto>>
{
    public long? Id { get; set; }

    public string? UserName { get; set; }

    public string? Surname { get; set; }

    public string? GivenName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? Age { get; set; }

    public int? Sex { get; set; }

    public string? Remark { get; set; }

    public long? DeptId { get; set; }

    public List<string> Roles { get; set; } = [];

    public override List<UserViewDto> Result { get; set; }
    public UserListQuery()
    {
    }
}