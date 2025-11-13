using AntDesign.ProLayout;
using LzqNet.Caller.Auth;
using LzqNet.Caller.Extensions;

namespace LzqNet.ApiGateway.Dashboard.Extensions;

public static class YarpDashboardExtensions
{
    public static void AddYarpDashboard(this IHostApplicationBuilder builder)
    {
        // 添加 HttpClient 服务
        builder.Services.AddHttpClient();
        builder.Services.AddAutoInject();
        builder.AddCustomCaller();

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddAntDesign();
        //builder.Services.AddScoped(async sp =>
        //{
        //    var apiUrl = builder.Configuration.GetSection("Services:apigateway")
        //               .Get<string>() ?? throw new InvalidOperationException($"未找到配置项:Services:apigateway");
        //    var httpClient = new HttpClient()
        //    {
        //        BaseAddress = new Uri(apiUrl)
        //    };
        //    var authCaller = sp.GetRequiredService<AuthCaller>();
        //    List<DefaultAccountOption> defaultAccountOptions =
        //        builder.Configuration.GetSection("DefaultAccounts").Get<List<DefaultAccountOption>>() ?? [];
        //    if (defaultAccountOptions.Count > 0)
        //    {
        //        var loginInfo = await authCaller.Login(new UserLoginDto(defaultAccountOptions[0].UserName, defaultAccountOptions[0].Password));
        //        if (loginInfo != null)
        //        {
        //            // 2. 将Token添加到请求头
        //            httpClient.DefaultRequestHeaders.Authorization =
        //                new AuthenticationHeaderValue("Bearer", loginInfo.AccessToken);
        //        }
        //    }
        //    return httpClient;
        //});
        builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));
        builder.Services.AddLocalization();

    }
    public static void MapYarpDashboard(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();
        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
    }
}
