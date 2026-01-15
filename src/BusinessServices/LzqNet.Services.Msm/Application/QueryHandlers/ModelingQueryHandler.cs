using LzqNet.Caller.Common.Contracts;
using LzqNet.Caller.Msm.Modeling.Model.Queries;
using LzqNet.Services.Msm.Domain.Entities.Modeling;
using LzqNet.Services.Msm.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using System.Linq.Expressions;

namespace LzqNet.Services.Msm.Application.QueryHandlers;

public class ModelingQueryHandler(IModelingRepository modelingRepository)
{
    private readonly IModelingRepository _modelingRepository = modelingRepository;

    [EventHandler]
    public async Task GetListHandleAsync(ModelingSelectListQuery query)
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
        var list = (await _modelingRepository.GetListAsync(condition)).Select(a => new SelectViewDto
        {
            Label = a.Description,
            Value = a.ModelingName,
        }).ToList();
        query.Result = list;
    }
}