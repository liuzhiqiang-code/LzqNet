using Masa.BuildingBlocks.Data;
using SqlSugar;

namespace LzqNet.Caller.Common.Contracts;

public class BaseFullEntity : IBaseFullEntity
{
    [SugarColumn(ColumnName = "id",IsPrimaryKey = true)]
    public long Id { get; private set; } = IdGeneratorFactory.SnowflakeGenerator.NewId();

    [SugarColumn(ColumnName = "creator")]
    public long Creator { get; set; }

    [SugarColumn(ColumnName = "creation_time")]
    public DateTime CreationTime { get; set; }

    [SugarColumn(ColumnName = "modifier")]
    public long Modifier { get; set; }

    [SugarColumn(ColumnName = "modification_time")]
    public DateTime ModificationTime { get; set; }

    [SugarColumn(ColumnName = "is_deleted")]
    public bool IsDeleted { get; set; }
}

public interface IBaseFullEntity : IEntity
{
    public long Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public long Modifier { get; set; }

    public DateTime ModificationTime { get; set; }

    public bool IsDeleted { get; set; }
}