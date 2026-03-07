using LzqNet.Common.Contracts;
using LzqNet.System.Contracts.Dept;
using LzqNet.System.Contracts.Dept.Queries;
using LzqNet.System.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

namespace LzqNet.System.Application.QueryHandlers;

public class DeptQueryHandler(IDeptRepository deptRepository)
{
    private readonly IDeptRepository _deptRepository = deptRepository;

    [EventHandler]
    public async Task GetListHandleAsync(DeptListQuery query)
    {
        // 获取所有部门数据
        var allDepts = (await _deptRepository.GetListAsync())
            .ToList()
            .Map<List<DeptViewDto>>();

        // 构建树形结构
        query.Result = BuildDeptTree(allDepts, null);
    }

    // 递归构建部门树
    private List<DeptViewDto> BuildDeptTree(List<DeptViewDto> allDepts, long? parentId)
    {
        return allDepts
            .Where(d => d.Pid == parentId)
            .Select(d => new DeptViewDto
            {
                Id = d.Id,
                Pid = d.Pid,
                Name = d.Name,
                Status = d.Status,
                Remark = d.Remark,
                Children = BuildDeptTree(allDepts, d.Id) // 递归处理子节点
            })
            .ToList();
    }


    [EventHandler]
    public async Task GetPageHandleAsync(DeptPageQuery query)
    {
        RefAsync<int> total = 0;
        var pageList = await _deptRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<DeptViewDto>>();
        query.Result = new PageList<DeptViewDto>(result, total);
    }
}
