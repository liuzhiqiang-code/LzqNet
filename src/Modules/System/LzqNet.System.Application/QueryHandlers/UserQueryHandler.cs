using LzqNet.Common.Contracts;
using LzqNet.System.Contracts.User;
using LzqNet.System.Contracts.User.Queries;
using LzqNet.System.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

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
        RefAsync<int> total = 0;
        var pageList = await _userRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<UserViewDto>>();
        query.Result = new PageList<UserViewDto>(result, total);
    }
}