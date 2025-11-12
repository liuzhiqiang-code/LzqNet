using AntDesign.TableModels;
using LzqNet.Caller.ApiGateway.Contracts;
using LzqNet.Caller.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace LzqNet.ApiGateway.Dashboard.Pages;

public partial class ProxyConfig
{
    [Inject] public MessageService MessageService { get; set; }
    [Inject] public ModalService ModalService { get; set; }
    [Inject] public ConfirmService ComfirmService { get; set; }
    [Inject] public ApiGatewayCaller ApiGatewayCaller { get; set; }


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
        _pageList = proxyConfigModel.Routes;
        //_total = result.Data.TotalCount.ToInt32();
        _tableLoading = false;
    }

    async Task OnChange(QueryModel<RouteConfigModel> query)
    {
        //queryData.PageIndex = query.PageIndex;
        //queryData.PageSize = query.PageSize;
        _tableLoading = true;
        await HandleSearch();
    }

    private async Task Delete(RouteConfigModel row)
    {
       
    }

    List<UploadFileItem> fileList = new List<UploadFileItem>();
    private async void UploadFiles(InputFileChangeEventArgs e)
    {
        
    }

    private async Task<bool> ComfirmAsync(string message)
    {
        return await ComfirmService.Show(message, "Confirm", ConfirmButtons.YesNo, ConfirmIcon.Warning) == ConfirmResult.Yes;
    }
}
