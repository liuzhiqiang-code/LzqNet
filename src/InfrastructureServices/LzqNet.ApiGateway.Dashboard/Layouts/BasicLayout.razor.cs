using AntDesign.ProLayout;
using Microsoft.AspNetCore.Components;

namespace LzqNet.ApiGateway.Dashboard.Layouts;

public partial class BasicLayout : LayoutComponentBase, IDisposable
{
    private MenuDataItem[] _menuData;

    [Inject] private ReuseTabsService TabService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _menuData = new[] {
             new MenuDataItem
            {
                Path = $"/proxyConfig",
                Name = "网关管理",
                Key = "网关管理",
                Icon = "smile",
            },
        };
    }

    void Reload()
    {
        TabService.ReloadPage();
    }

    public void Dispose()
    {

    }

}
