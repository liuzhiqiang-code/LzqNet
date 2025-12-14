using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.Menu.Queries;

public record MenuNameExistsQuery : Query<bool?>
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public override bool? Result { get; set; }
    public MenuNameExistsQuery(long? id, string? name)
    {
        Id = id;
        Name = name;
    }
}
