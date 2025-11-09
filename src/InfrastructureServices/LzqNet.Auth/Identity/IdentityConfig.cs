using Duende.IdentityServer.Models;

namespace LzqNet.Auth.Identity;

public class IdentityConfig
{
    // 定义身份资源（Identity Resources）
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        ];

    // 定义 API 范围（API Scopes）
    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope("common") // 统一 scope
        ];

    // 定义 API 资源（API Resources）
    public static IEnumerable<ApiResource> ApiResources =>
        [
            new ApiResource("common", "公用Api")
            {
                Scopes = { "common" }
            }
        ];

    // 定义客户端（Clients）
    public static IEnumerable<Client> Clients =>
        [
            // 客户端模式（Client Credentials）
            new Client
            {
                ClientId = "client",
                ClientName = "客户端模式",
                AllowedGrantTypes = GrantTypes.ClientCredentials, // 客户端凭证模式
                ClientSecrets =
                {
                    new Secret("secret".Sha256()) // 客户端密钥
                },
                AllowedScopes = { "common" } // 允许的 API 访问范围
            },
 
            // 资源所有者密码模式（Resource Owner Password）
            new Client
            {
                ClientId = "register.client",
                ClientName = "资源所有者",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // 用户名密码模式
                ClientSecrets =
                {
                    new Secret("secret".Sha256()) // 客户端密钥
                },
                AllowedScopes = { "common" } // 允许的 API 访问范围
            }
        ];
}
