using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;
using LzqNet.Caller.Msm.Contracts.Dept;
using LzqNet.Caller.Msm.Contracts.Dept.Queries;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Application.QueryHandlers;

public class DeptQueryHandler(IDeptRepository deptRepository)
{
    private readonly IDeptRepository _deptRepository = deptRepository;

    [EventHandler]
    public async Task GetListHandleAsync(DeptGetListQuery query)
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
        PaginatedOptions paginatedOptions = new() {
            Page =  query.SearchDto.Page,
            PageSize = query.SearchDto.PageSize
        };
        var pageList = await _deptRepository.GetPaginatedListAsync(paginatedOptions);
        query.Result = new PaginatedListBase<DeptViewDto>
        {
            Result = pageList.Result.Map<List<DeptViewDto>>(),
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }
}
