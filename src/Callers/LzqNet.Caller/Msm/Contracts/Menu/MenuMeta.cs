using System.ComponentModel;

namespace LzqNet.Caller.Msm.Contracts.Menu;

/// <summary>
/// 菜单元数据
/// </summary>
public class MenuMeta
{
    public string? ActiveIcon { get; set; }
    public string? ActivePath { get; set; }
    public bool? AffixTab { get; set; }
    public int? AffixTabOrder { get; set; }
    public string? Badge { get; set; }
    public string? BadgeType { get; set; }
    public string? BadgeVariant { get; set; }
    public bool? HideChildrenInMenu { get; set; }
    public bool? HideInBreadcrumb { get; set; }
    public bool? HideInMenu { get; set; }
    public bool? HideInTab { get; set; }
    public string? Icon { get; set; }
    public string? IframeSrc { get; set; }
    public bool? KeepAlive { get; set; }
    public string? Link { get; set; }
    public int? MaxNumOfOpenTab { get; set; }
    public bool? NoBasicLayout { get; set; }
    public bool? OpenInNewWindow { get; set; }
    public int? Order { get; set; }
    public Dictionary<string, object>? Query { get; set; }
    public string? Title { get; set; }
}

/// <summary>
/// 徽标颜色集合
/// </summary>
public enum BadgeVariant
{
    Default,
    Destructive,
    Primary,
    Success,
    Warning
}

/// <summary>
/// 徽标类型集合
/// </summary>
public enum BadgeType
{
    Dot,
    Normal
}

/// <summary>
/// 菜单类型集合
/// </summary>
public enum MenuType
{
    [Description("目录")]
    Catalog,
    [Description("菜单")]
    Menu,
    [Description("内嵌")]
    Embedded,
    [Description("外链")]
    Link,
    [Description("按钮")]
    Button
}