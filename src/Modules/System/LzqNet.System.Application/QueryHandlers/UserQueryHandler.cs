using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;
using LzqNet.System.Contracts.User.Queries;
using LzqNet.System.Contracts.User;
using LzqNet.System.Domain.IRepositories;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Application.QueryHandlers;

public class UserQueryHandler(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    [EventHandler]
    public async Task GetListHandleAsync(UserListQuery query)
    {
        var list = (await _userRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<UserViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(UserPageQuery query)
    {
        var paginatedOptions = new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize
        };
        var pageList = await _userRepository.GetPaginatedListAsync(paginatedOptions);
        var result = pageList.Result.Map<List<UserViewDto>>();
        query.Result = new PaginatedListBase<UserViewDto>
        {
            Result = result,
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }
}