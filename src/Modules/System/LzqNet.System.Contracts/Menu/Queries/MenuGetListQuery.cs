using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.System.Contracts.Menu.Queries;

public record MenuListQuery : Query<List<MenuViewDto>>
{
    /// <summary>
    /// 
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public long? Pid { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Authcode { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Meta { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Redirect { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public long? Creator { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? Creationtime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public long? Modifier { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? Modificationtime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public byte? Isdeleted { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? Status { get; set; }

    public override List<MenuViewDto> Result { get; set; }
    public MenuListQuery()
    {
    }
}
