using LzqNet.Caller.Msm.Contracts.User;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.Account.Queries;

public record UserInfoQuery : Query<UserInfoViewDto>
{
    public override UserInfoViewDto Result { get; set; }
    public UserInfoQuery()
    {
    }
}