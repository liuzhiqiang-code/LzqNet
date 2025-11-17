using LzqNet.Caller.ApiGateway.Contracts;
using LzqNet.Caller.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;

namespace LzqNet.ApiGateway.Dashboard.Pages;

public partial class ClusterConfigModal
{
    [Inject] public MessageService MessageService { get; set; }
    [Inject] public ModalService ModalService { get; set; }
    [Inject] public ConfirmService ComfirmService { get; set; }
    [Inject] public ApiGatewayCaller ApiGatewayCaller { get; set; }

    [Parameter]
    public ProxyConfigModel ProxyConfigModel { get; set; } = new ProxyConfigModel();
    [Parameter]
    public ClusterConfigModel FormData { get; set; } = new ClusterConfigModel();


    [Parameter]
    public bool Visible { get; set; } = false;
    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }


    [Parameter]
    public EventCallback<ClusterConfigModel> DataChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override void OnParametersSet()
    {
        EditDestinations.Clear();
        if (FormData?.Destinations != null)
        {
            foreach (var item in FormData.Destinations)
            {
                item.Value.Address = item.Value.Address.TrimStart("http://");
                EditDestinations.Add(new EditDestinationItem
                {
                    Key = item.Key.TrimStart(FormData.ClusterId + "/"),
                    Value = item.Value
                });
            }
        }
    }


    private void OnFinish(EditContext editContext)
    {
    }

    private void OnFinishFailed(EditContext editContext)
    {
    }

    private async Task HandleOk(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        SyncToFormDataTransforms();
        if (DataChanged.HasDelegate)
        {
            // 调用 EventCallback，这会执行父组件中绑定的 HandleDataChangedAsync 方法
            await DataChanged.InvokeAsync(FormData);
        }
    }
    private async Task HandleCancel()
    {
        if (VisibleChanged.HasDelegate)
        {
            // 调用 EventCallback，这会执行父组件中绑定的 HandleDataChangedAsync 方法
            await VisibleChanged.InvokeAsync(false);
        }
    }

    private async Task<bool> ComfirmAsync(string message)
    {
        return await ComfirmService.Show(message, "Confirm", ConfirmButtons.YesNo, ConfirmIcon.Warning) == ConfirmResult.Yes;
    }

    private void AddNewItem()
    {
        EditDestinations.Add(new EditDestinationItem
        {
            Key = "",
            Value = new DestinationConfigModel()
        });
    }
    private void RemoveItem(EditDestinationItem item)
    {
        if (EditDestinations.Count == 1)
            return;
        EditDestinations.Remove(item);
    }
    private void SyncToFormDataTransforms(ChangeEventArgs e = null)
    {
        if (FormData.Destinations == null)
        {
            FormData.Destinations = new Dictionary<string, DestinationConfigModel>();
        }
        else
        {
            FormData.Destinations.Clear();
        }

        var dict = new Dictionary<string, DestinationConfigModel>();
        foreach (var item in EditDestinations)
        {
            item.Value.Address = "http://" + item.Value.Address;
            dict[FormData.ClusterId + "/" + item.Key] = item.Value;
        }
        FormData.Destinations = dict;
    }
}

// 自定义可编辑键值对类
public class EditDestinationItem
{
    public string Key { get; set; } = "";
    public DestinationConfigModel Value { get; set; }
}