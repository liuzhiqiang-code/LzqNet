namespace LzqNet.Common.Contracts;

public record PageList<TEntity>(
    List<TEntity> Result,
    long Total
) where TEntity : class
{
}