using LzqNet.Caller.ApiGateway.Contracts;
using LzqNet.Caller.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace LzqNet.ApiGateway.Dashboard.Pages;

public partial class RouteConfigModal
{
    [Inject] public MessageService MessageService { get; set; }
    [Inject] public ModalService ModalService { get; set; }
    [Inject] public ConfirmService ComfirmService { get; set; }
    [Inject] public ApiGatewayCaller ApiGatewayCaller { get; set; }

    [Parameter]
    public ProxyConfigModel ProxyConfigModel { get; set; } = new ProxyConfigModel();
    [Parameter]
    public RouteConfigModel FormData { get; set; } = new RouteConfigModel();


    [Parameter]
    public bool Visible { get; set; } = false;
    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }


    [Parameter]
    public EventCallback<RouteConfigModel> DataChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override void OnParametersSet()
    {
        if (ProxyConfigModel?.Clusters != null)
            _clusterOptions = ProxyConfigModel.Clusters.Select(c => c.ClusterId).ToArray();
        else
            _clusterOptions = [];
        // 当父组件修改FormData时同步更新selectedMethods
        if (FormData?.Match?.Methods != null)
            selectedMethods = FormData.Match.Methods.ToArray();
        else
            selectedMethods = [];

        EditableItems.Clear();
        if (FormData?.Transforms != null) 
        {
            foreach (var item in FormData.Transforms)
            {
                var EditableItem = new List<EditableKeyValuePair>();
                foreach (var keyValuePair in item)
                    EditableItem.Add(new EditableKeyValuePair
                    {
                        Key = keyValuePair.Key,
                        Value = keyValuePair.Value
                    });
                EditableItems.Add(EditableItem);
            }
        }
    }

    private void OnSelectedMethodsChanged(string[] values)
    {
        selectedMethods = values;
        if (FormData?.Match != null)
            FormData.Match.Methods = values.ToList();
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
        EditableItems.Add(new List<EditableKeyValuePair> {
            new EditableKeyValuePair { Key = "", Value = "" }
        });
    }
    private void RemoveItem(List<EditableKeyValuePair> keyValuePairs, EditableKeyValuePair keyValuePair)
    {
        keyValuePairs.Remove(keyValuePair);
        if(keyValuePairs.Count == 0)
            EditableItems.Remove(keyValuePairs);
        if (EditableItems.Count == 0)
            AddNewItem();
    }

    private void SyncToFormDataTransforms(ChangeEventArgs e = null)
    {
        if (FormData.Transforms == null)
        {
            FormData.Transforms = new List<Dictionary<string, string>>();
        }
        else
        {
            FormData.Transforms.Clear();
        }

        foreach (var editableList in EditableItems)
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in editableList)
            {
                dict[item.Key] = item.Value;
            }
            FormData.Transforms.Add(dict);
        }
    }

}


// 自定义可编辑键值对类
public class EditableKeyValuePair
{
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}