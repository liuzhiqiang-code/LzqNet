## 支持的事务类型

事件分为**进程内事件**与**集成事件**的发布：

- **进程内事件**：基于内存的事件分发机制
  参考：[MASA Stack - 本地事件](https://docs.masastack.com/framework/building-blocks/dispatcher/local-event)
- **集成事件**：默认采用发件箱模式 + RabbitMQ，事件信息持久化到 `integration_event_log` 表，通过发件箱确保最终一致性
  参考：[MASA Stack - 集成事件](https://docs.masastack.com/framework/building-blocks/dispatcher/integration-event)

 **⚠️ 注意事项**：EventBus默认Scoped声明周期,一个请求中有多线程发布命令，可能导致`Repository`共享DB上下文实例,导致事务不完整,如果有这种场景可以将EventBus注入周期改为`Transient`
``` csharp
services.AddValidatorsFromAssemblies(assemblies)
    .AddEventBus(assemblies, ServiceLifetime.Transient, eventBusBuilder =>
    {
        eventBusBuilder.UseMiddleware(typeof(ValidatorEventMiddleware<>));
        eventBusBuilder.UseMiddleware(typeof(SugarUowEventMiddleware<>));
    });
```

### 事件总线配置示例

``` csharp
// 进程内事件 - 可添加自定义中间件
services.AddValidatorsFromAssemblies(assemblies)
    .AddEventBus(assemblies, eventBusBuilder =>
    {
        eventBusBuilder.UseMiddleware(typeof(ValidatorEventMiddleware<>));
        eventBusBuilder.UseMiddleware(typeof(SugarUowEventMiddleware<>));
    });

// 集成事件 - 使用 RabbitMQ + 事件日志
services.AddIntegrationEventBus(option =>
{
    option.UseRabbitMq().UseEventLog();
});
```

---

## 事务处理模式

### 1. 单事件事务

单个事件独立执行，通过 `[UnitOfWork]` 特性控制事务行为。

**特性说明**：`[UnitOfWork]` 默认隔离级别为 ReadCommitted，可自己加参数改隔离级别

``` csharp
[UnitOfWork(IsolationLevel.ReadCommitted)]
public record TestContentCreateCommand : Command
{}

[OpenApiTag("TestContent", Description = "单事件发布")]
[RoutePattern(pattern: "createByMoreRequest", true)]
public async Task<AdminResult> CreateByMoreRequestAsync([FromBody] TestContentCreateCommand command)
{
    await EventBus.PublishAsync(command);
    return AdminResult.Success();
}
```

---

### 2. 单事件多库事务

**场景**：一个事件中操作多个数据库
**原理**：利用 SqlSugar 的多库事务能力，统一管理多个数据库上下文的事务生命周期  SqlSugarClient对象中`List<SugarTenant>`一起开启关闭回滚事务 

``` csharp
// 业务层直接发布事件，无需额外事务控制
await EventBus.PublishAsync(command);
```

📚 参考：[SqlSugar 多库事务](https://www.donet5.com/Home/Doc?typeId=1183)

---

### 3. 多个事件 + 单数据库 + 同一事务

**场景**：事件 A 和事件 B 操作同一个数据库，需保证原子性
**原理**：若当前线程已开启事务，事件中间件自动跳过新事务创建，复用现有事务

``` csharp
var dbResult = await SqlSugarClient.AsTenant().UseTranAsync(async () =>
{
    await EventBus.PublishAsync(commandA);
    await EventBus.PublishAsync(commandB);
});
```

---

### 4. 多个事件 + 多数据库 + 同一事务

**场景**：事件 A 操作 `db1`，事件 B 操作 `db2`，需保证跨库原子性
**实现**：基于 SqlSugar 的多库事务管理器

``` csharp
var dbResult = await SqlSugarClient.AsTenant().UseTranAsync(async () =>
{
    await EventBus.PublishAsync(commandA);
    await EventBus.PublishAsync(commandB);
});
```

---

### 5. 多线程 + 单个事件 + 多数据库 + 独立事务

**场景**：同一请求内多个 Task 并发执行同一事件，每个事件操作多库，各自独立事务

``` csharp
int threadCount = 10;
var tasks = new List<Task>();
var results = new ConcurrentBag<ThreadResult>();

for (int i = 0; i < threadCount; i++)
{
    tasks.Add(Task.Run(async () =>
    {
        await EventBus.PublishAsync(threadCommand);
        results.Add(new ThreadResult());
    }));
}

await Task.WhenAll(tasks);
```

---

### 6. 多线程 + 多个事件 + 多数据库 + 独立事务

**场景**：并发执行多个 Task，每个 Task 内发布事件 A（操作 `db1`）和事件 B（操作 `db2`），Task 间事务隔离

``` csharp
int threadCount = 10;
var tasks = new List<Task>();
var results = new ConcurrentBag<ThreadResult>();

for (int i = 0; i < threadCount; i++)
{
    tasks.Add(Task.Run(async () =>
    {
        var dbResult = await SqlSugarClient.AsTenant().UseTranAsync(async () =>
        {
            await EventBus.PublishAsync(threadCommandA);
            await EventBus.PublishAsync(threadCommandB);
        });
        results.Add(new ThreadResult());
    }));
}

await Task.WhenAll(tasks);
```