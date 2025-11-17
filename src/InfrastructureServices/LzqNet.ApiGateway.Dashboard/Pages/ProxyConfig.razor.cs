using AntDesign.TableModels;
using LzqNet.Caller.ApiGateway.Contracts;
using LzqNet.Caller.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;

namespace LzqNet.ApiGateway.Dashboard.Pages;

public partial class ProxyConfig
{
    [Inject] public MessageService MessageService { get; set; }
    [Inject] public ModalService ModalService { get; set; }
    [Inject] public ConfirmService ComfirmService { get; set; }
    [Inject] public ApiGatewayCaller ApiGatewayCaller { get; set; }

    private ProxyConfigModel ProxyConfigModel { get; set; }


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private void OnFinish(EditContext editContext)
    {
    }

    private void OnFinishFailed(EditContext editContext)
    {
    }

    private async Task HandleSearch()
    {
        _tableLoading = true;
        var proxyConfigModel = await ApiGatewayCaller.GetConfig();
        if (proxyConfigModel == null)
        {
            await ComfirmAsync("123");
            return;
        }
        ProxyConfigModel = proxyConfigModel;
        _pageList = proxyConfigModel.Routes;
        _tableLoading = false;
    }

    async Task OnChange(QueryModel<RouteConfigModel> query)
    {
        _tableLoading = true;
        await HandleSearch();
    }
    private async Task EditRoute(RouteConfigModel row)
    {
        ShowRouteModal(row);
    }
    private async Task EditCluster(RouteConfigModel row)
    {
        var clusterConfig = ProxyConfigModel.Clusters.Find(c => c.ClusterId == row.ClusterId)!;
        ShowClusterModal(clusterConfig);
    }
    private async Task Delete(RouteConfigModel row)
    {
       
    }

    private async Task<bool> ComfirmAsync(string message)
    {
        return await ComfirmService.Show(message, "Confirm", ConfirmButtons.YesNo, ConfirmIcon.Warning) == ConfirmResult.Yes;
    }

    private void ShowRouteModal(RouteConfigModel row)
    {
        _routeVisible = true;
        CurrentRouteConfig = row;
        _proxyConfigModel = ProxyConfigModel;
    }
    private void ShowClusterModal(ClusterConfigModel row)
    {
        _clusterVisible = true;
        CurrentClusterConfig = row;
        _proxyConfigModel = ProxyConfigModel;
    }

    private async Task HandleRouteDataChangedAsync(RouteConfigModel model)
    {
        foreach (var item in ProxyConfigModel.Routes)
        {
            if (item.RouteId == model.RouteId)
            {
                item.Order = model.Order;
                item.Match = model.Match;
                item.RateLimiterPolicy = model.RateLimiterPolicy;
                item.Metadata = model.Metadata;
                item.Transforms = model.Transforms;
            }
        }
        await ComfirmAsync(JsonConvert.SerializeObject(ProxyConfigModel));
        _routeVisible = false;
    }

    private async Task HandleClusterDataChangedAsync(ClusterConfigModel model)
    {
        foreach (var item in ProxyConfigModel.Clusters)
        {
            if (item.ClusterId == model.ClusterId)
            {
                item.ClusterId = model.ClusterId;
            }
        }
        await ComfirmAsync(JsonConvert.SerializeObject(ProxyConfigModel));
        _clusterVisible = false;
    }
}
