using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Modeling.Model.Queries;


public record ModelDataQuery : Query<ModelViewDto>
{
    public long Id { get; set; }
    public override ModelViewDto Result { get; set; }
    public ModelDataQuery(long id)
    {
        Id = id;
    }
}
