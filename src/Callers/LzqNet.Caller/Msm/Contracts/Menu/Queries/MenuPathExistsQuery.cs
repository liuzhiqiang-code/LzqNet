using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.Menu.Queries;

public record MenuPathExistsQuery : Query<bool?>
{
    public long? Id { get; set; }
    public string? Path { get; set; }
    public override bool? Result { get; set; }
    public MenuPathExistsQuery(long? id, string? path)
    {
        Id = id;
        Path = path;
    }
}
