using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.Common.Contracts;

public abstract record PageQuery<TResult> : Query<PaginatedListBase<TResult>> where TResult : class
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    protected PageQuery() : base() { }
}