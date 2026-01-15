using LzqNet.Caller.Common.Contracts;
using LzqNet.Caller.Msm.Modeling.Model;
using LzqNet.Caller.Msm.Modeling.Model.Queries;
using LzqNet.Services.Msm.Domain.Entities.Modeling;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using System;
using System.Linq.Expressions;

namespace LzqNet.Services.Msm.Application.QueryHandlers.Modeling;

public class ModelQueryHandler(IRepository<ModelingEntity, long> repository)
{
    private readonly IRepository<ModelingEntity, long> _repository = repository;

    [EventHandler]
    public async Task GetSelectListHandleAsync(ModelSelectListQuery query)
    {
        var searchDto = query.SearchDto;
        Expression<Func<ModelingEntity, bool>> condition = a => true;
        if (!searchDto.Keyword.IsNullOrWhiteSpace())
        {
            condition.And(
                a => a.ModelingName.Equals(searchDto.Keyword)
                || a.Description.Equals(searchDto.Keyword)
            );
        }
        var list = (await _repository.GetListAsync(condition)).Select(a => new SelectViewDto
        {
            Label = a.ModelingName,
            Value = a.ModelingId.ToString(),
        }).ToList();
        query.Result = list;
    }

    [EventHandler]
    public async Task GetDataHandleAsync(ModelDataQuery query)
    {
        var id = query.Id;
        var data = await _repository.FindAsync(a=>a.Id.Equals(query.Id));
        if (data != null)
            query.Result = data.Map<ModelViewDto>();
    }
}