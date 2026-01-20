using LzqNet.Caller.Msm.Contracts.Menu;
using LzqNet.Caller.Msm.Contracts.Menu.Queries;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using System.Linq.Expressions;

namespace LzqNet.Services.Msm.Application.QueryHandlers;

public class MenuQueryHandler(IMenuRepository MenuRepository)
{
    private readonly IMenuRepository _MenuRepository = MenuRepository;

    [EventHandler]
    public async Task NameExistsHandleAsync(MenuNameExistsQuery query)
    {
        if (query.Name.IsNullOrWhiteSpace())
        {
            query.Result = false;
            return;
        }
        Expression<Func<MenuEntity, bool>> condition = a => true;
        condition = condition.And(query.Id.HasValue, a => !a.Id.Equals(query.Id));
        condition = condition.And(!query.Name.IsNullOrWhiteSpace(), a => a.Name.Equals(query.Name));
        var count = await _MenuRepository.CountAsync(condition);
        query.Result = count > 0;
    }

    [EventHandler]
    public async Task PathExistsHandleAsync(MenuPathExistsQuery query)
    {
        if (query.Path.IsNullOrWhiteSpace())
        {
            query.Result = false;
            return;
        }
        Expression<Func<MenuEntity, bool>> condition = a => true;
        condition = condition.And(query.Id.HasValue, a => !a.Id.Equals(query.Id));
        condition = condition.And(!query.Path.IsNullOrWhiteSpace(), a => a.Path!.Equals(query.Path));
        var count = await _MenuRepository.CountAsync(condition);
        query.Result = count > 0;
    }


    [EventHandler]
    public async Task GetListHandleAsync(MenuGetListQuery query)
    {
        // 获取所有菜单数据
        var allMenus = (await _MenuRepository.GetListAsync())
            .ToList()
            .Map<List<MenuViewDto>>();

        // 构建树形结构
        query.Result = BuildMenuTree(allMenus, null);
    }

    // 递归构建部门树
    private List<MenuViewDto> BuildMenuTree(List<MenuViewDto> allMenus, long? parentId)
    {
        return allMenus
            .Where(d => d.Pid == parentId)
            .Select(d => new MenuViewDto
            {
                Id = d.Id,
                Pid = d.Pid,
                Name = d.Name,
                AuthCode = d.AuthCode,
                Component = d.Component,
                Meta = d.Meta,
                Path = d.Path,
                Redirect = d.Redirect,
                Type = d.Type,
                Children = BuildMenuTree(allMenus, d.Id) // 递归处理子节点
            })
            .ToList();
    }
}
