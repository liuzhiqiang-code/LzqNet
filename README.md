# LzqNet 微服务框架

## 📁 项目结构

``` text
Solution Items/          # 脚本及框架配置文件
src/
├── BusinessServices/    # 启动程序项目
│   └── LzqNet.Services.Msm/  # 主启动程序（引用模块Application程序集+配置）
├── InfrastructureServices/   # 框架相关项目
│   ├── Daily.Carp/           # YARP二次封装的网关核心
│   ├── Daily.Carp.Provider.Consul/  # Consul服务发现
│   ├── LzqNet.ApiGateway/    # 网关（YARP+Consul+限流+遥测+Swagger集成）
│   ├── LzqNet.Auth/          # 授权中心（基于Duende.IdentityServer）
│   ├── LzqNet.Common/        # 公用基础库（工具库+基础微服务SDK）
│   ├── LzqNet.Extensions/     # 扩展库（启动程序中间件配置）
│   ├── Masa.BuildingBlocks.Dispatcher.IntegrationEvents/  # 事件总线抽象
│   └── Masa.Contrib.Dispatcher.IntegrationEvents/         # 事件总线实现（RabbitMQ+发件箱模式）
├── Modules/             # 业务模块
│   └── Template/        # 示例模块
│       ├── LzqNet.Template.Application/  # 应用层（API请求+业务逻辑+CQRS）
│       ├── LzqNet.Template.Consumer/     # 消费者层（队列消费）
│       ├── LzqNet.Template.Contracts/    # 契约层（输入/输出报文+服务SDK）
│       └── LzqNet.Template.Domain/       # 领域层（实体+仓储）
└── docker-compose/      # Docker编排配置（Nginx+Redis+RabbitMQ+应用程序）
```

## 🚀 启动方式

### 1️⃣ 微服务模式（默认）

**设置**：将 `docker-compose` 设为启动项目

**配置修改**：

``` csharp
// Program.cs
builder.AddApplicationConfiguration().AddCustomConsul();  // 保留Consul配置读取
```

**appsettings.json 配置**：

``` json
{
  "GlobalConfig": {
    "UseAuth": true,        // true: 使用LzqNet.Auth授权中心
    "UseSwagger": false     // false: 生成OpenAPI文档供网关集成
  },
  "Jwt": {
    "Audience": "common",
    "Authority": "http://lzqnet.auth:8080",  // 授权中心地址
    "RequireHttpsMetadata": false
  }
}
```

### 2️⃣ 单体模式

**配置修改**：

``` csharp
// Program.cs
builder.AddApplicationConfiguration();  // 注释 AddCustomConsul()
```

**appsettings.json 配置**：

``` json
{
  "GlobalConfig": {
    "UseAuth": false,       // false: 使用单体JWT授权
    "UseSwagger": true      // 直接启用Swagger
  },
  "Jwt": {
    "Authority": "Issuer",
    "Audience": "common",
    "RequireHttpsMetadata": false,
    "Issuer": "Issuer",
    "AccessExpiration": "7200",
    "Secret": "your-very-long-secret-key-that-is-32-characters-minimum-here LzqNet Secret 123numberSecret",
    "RefreshSecret": "your-very-long-secret-key-that-is-32-characters-minimum-here LzqNet RefreshSecret 123numberSecret"
  }
}
```

## ✨ 核心特性

- **API网关**：基于YARP，集成Consul服务发现、限流、遥测
- **授权认证**：支持Duende IdentityServer集中授权或单体JWT
- **配置中心**：支持Consul配置中心（微服务模式）
- **事件总线**：基于MasaFramework，支持RabbitMQ+发件箱模式
- **监控集成**：集成健康检查、Serilog日志、遥测
- **模块化设计**：清晰的DDD分层架构

## 🛠 技术栈

- .NET 6/8/10
- YARP + Consul
- Duende IdentityServer
- SqlSugar ORM
- MasaFramework
- Serilog
- Docker + Docker Compose

---

> 📌 **文档地址**：[https://liuzhiqiang-code.github.io/LzqNet/#/](https://liuzhiqiang-code.github.io/LzqNet/#/)
> 📌 **微软AI文档地址**：[https://learn.microsoft.com/zh-cn/agent-framework/overview/?pivots=programming-language-csharp](https://learn.microsoft.com/zh-cn/agent-framework/overview/?pivots=programming-language-csharp)