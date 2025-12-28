using Masa.BuildingBlocks.Data;
using Masa.BuildingBlocks.Ddd.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Caller.Common.Contracts;

public class BaseFullEntity: IEntity<long>
{
    [Column("id")]
    public long Id { get; private set; }

    [Column("creator")]
    public long Creator { get; set; }

    [Column("creation_time")]
    public DateTime CreationTime { get; set; }

    [Column("modifier")]
    public long Modifier { get; set; }

    [Column("modification_time")]
    public DateTime ModificationTime { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    public IEnumerable<(string Name, object Value)> GetKeys()
    {
        return [(nameof(Id), (object)Id)];
    }
    public BaseFullEntity()
    {
        Id = IdGeneratorFactory.SnowflakeGenerator.NewId();
    }
}
