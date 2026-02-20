using LzqNet.Common.Callers.Auth.Contracts;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.System.Contracts.Account.Queries;

public record UserInfoQuery : Query<UserInfoViewDto>
{
    public override UserInfoViewDto Result { get; set; }
    public UserInfoQuery()
    {
    }
}