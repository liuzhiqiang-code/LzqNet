using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Common.Contracts;

public abstract record PageQuery<TResult> : Query<PageList<TResult>> where TResult : class
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    protected PageQuery() : base() { }
}