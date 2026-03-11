using LzqNet.Common.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace LzqNet.Extensions.NSwag;

public static class NSwagExtensions
{
    public static WebApplicationBuilder AddLzqNSwag(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApiDocument(document =>
        {
            document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "请输入 Token，格式为: Bearer {your_token}"
            });

            // 让所有接口默认要求认证（或者根据需要配置）
            document.OperationProcessors.Add(
                new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });
        return builder;
    }

    public static void UseLzqNSwag(this IApplicationBuilder app)
    {
        GlobalConfig globalConfig = app.ApplicationServices.GetRequiredService<IOptions<GlobalConfig>>().Value;
        if (globalConfig.UseSwagger)
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
        }
        else
        {
            app.UseOpenApi();// 仍然启用 OpenAPI 中间件，但不启用 Swagger UI,OpenAPI的json需要授权，只有网关能访问到，外部无法访问到json接口
        }
    }
}
