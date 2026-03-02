using LzqNet.Test.Contracts.TestContent.Commands;
using LzqNet.Test.Contracts.TestContent.Queries;
using LzqNet.Test.Contracts.TestContentLog.Commands;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SqlSugar;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace LzqNet.Test.Application.Services;

/// <summary>
/// 各种并发情况下事务完整性的测试用例
/// </summary>
public class TestContentService : ServiceBase
{
    public TestContentService() : base("/api/v1/testContent") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();
    private ISqlSugarClient SqlSugarClient => GetRequiredService<ISqlSugarClient>();


    [OpenApiTag("TestContent", Description = "测试授权")]
    [Authorize]
    public IResult GetClaims([FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var claims = httpContextAccessor.HttpContext!.User.Claims.Select(claim =>
        {
            return new
            {
                claim.Type,
                claim.Value
            };
        });
        return Results.Ok(claims);
    }

    /*
     * 1.测试高并发新增数据到数据库     看有无数据丢失情况   无
     * 2.测试高并发修改数据到数据库     看有无脏读，重复读等情况
     * 服务直接调用   38 s   31.81 s   31.55 s
     * 事件总线无中间件  47.13 s   43.74 s  45.13 s
     * 参数验证中间件  46.73 s  47.39 s  47.42 s
     * 参数验证加单元事务中间件   165.67  148.66 s   148.52 s    事务完整
     * 4.测试高并发多个事件单元事务     事务完整
     * 
     * 5.测试执行sql超时  sql慢sql日志   超时处理机制  超时事务等
     * 6.死锁场景：A事务：更新表1->更新表2；B事务：更新表2->更新表1；高并发下验证死锁检测与重试机制
     * 8.嵌套事务场景：外层事务成功，内层事务失败，验证整体回滚 or 部分提交（REQUIRES_NEW效果）
     * 嵌套事务完整
     * 
     * 9.分布式事务模拟：两个独立数据源（或两个库），本地事务+远程调用，模拟部分成功部分失败
     * 同一个应用程序，两个不同数据库的事务
     * 
     * 10.补偿事务场景：先insert，后update同一批数据，高并发下事务穿插，验证最终一致性
     * 11.事件执行日志，慢sql日志，接口调用日志，服务器资源监控日志，业务链路日志
     * 12.事务中业务处理成功，消息发送失败，验证消息重试机制与事务一致性
     */

    #region 新增

    [OpenApiTag("TestContent", Description = "单事件发布")]
    [RoutePattern(pattern: "createByMoreRequest", true)]
    public async Task<AdminResult> CreateByMoreRequestAsync([FromBody] TestContentCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    // 事件发布调用
    // 无中间件  47.13 s   43.74 s  45.13 s
    // 参数验证中间件  46.73 s  47.39 s  47.42 s
    // 参数验证加单元事务中间件   165.67  148.66 s   148.52 s
    // 服务直接调用   38 s   31.81 s   31.55 s
    [OpenApiTag("TestContent", Description = "多线程单事件发布，单事件事务")]
    [RoutePattern(pattern: "createByMorenThread", true)]
    public async Task<AdminResult> CreateByMorenThreadAsync([FromBody] TestContentCreateCommand command)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        int threadCount = 10;
        var tasks = new List<Task>();
        var results = new ConcurrentBag<ThreadResult>();
        // 创建多个任务并发执行
        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i; // 避免闭包问题
            tasks.Add(Task.Run(async () =>
            {
                var result = new ThreadResult { ThreadId = threadId };
                try
                {
                    // 创建新的命令实例
                    var threadCommand = new TestContentCreateCommand
                    {
                        Name = $"{command.Name}_TrackId{traceId}_Thread{threadId}",
                        Remark = $"{command.Remark}_Thread{threadId}"
                    };
                    await EventBus.PublishAsync(threadCommand);

                    // 服务直接调用   38 s   31.81 s   31.55 s
                    // await GetRequiredService<TestContentCommandHandler>().CreateHandleAsync(threadCommand);

                    result.IsSuccess = true;
                    result.Message = "执行成功";
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = "执行失败";
                    result.Error = ex.ToString();
                }
                results.Add(result);
            }));
        }

        // 等待所有任务完成
        await Task.WhenAll(tasks);

        // 统计结果
        var summary = new
        {
            TotalThreads = threadCount,
            SuccessCount = results.Count(r => r.IsSuccess),
            FailCount = results.Count(r => !r.IsSuccess),
            SuccessRate = $"{Math.Round((double)results.Count(r => r.IsSuccess) / threadCount * 100, 2)}%",
            ExecuteTime = DateTime.Now,
            Details = results.OrderBy(r => r.ThreadId).ToList()
        };
        if (summary.SuccessCount == results.Count)
            return AdminResult.Success(summary);
        else
            return AdminResult.Fail($"部分线程执行失败", -1, summary);
    }

    [OpenApiTag("TestContent", Description = "多线程，多事件，每个事件单独事务")]
    [RoutePattern(pattern: "createByMorenThreadMoreEvent", true)]
    public async Task<AdminResult> CreateByMorenThreadMoreEventAsync([FromBody] TestContentCreateCommand command)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        int threadCount = 10;
        var tasks = new List<Task>();
        var results = new ConcurrentBag<ThreadResult>();
        // 创建多个任务并发执行
        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i; // 避免闭包问题
            tasks.Add(Task.Run(async () =>
            {
                var result = new ThreadResult { ThreadId = threadId };
                try
                {
                    // 创建新的命令实例
                    var threadCommand = new TestContentCreateCommand
                    {
                        Name = $"{command.Name}_TrackId{traceId}_Thread{threadId}",
                        Remark = $"{command.Remark}_Thread{threadId}"
                    };
                    // 事件发布调用
                    // 无中间件  47.13 s   43.74 s  45.13 s
                    // 参数验证中间件  46.73 s  47.39 s  47.42 s
                    // 参数验证加单元事务中间件   165.67  148.66 s   148.52 s
                    await EventBus.PublishAsync(threadCommand);

                    // 服务直接调用   38 s   31.81 s   31.55 s
                    // await GetRequiredService<TestContentCommandHandler>().CreateHandleAsync(threadCommand);

                    var threadUpdateCommand = new TestContentUpdateCommand
                    {
                        Name = $"{command.Name}_TrackId{traceId}_Thread{threadId}",
                        Remark = $"{command.Remark}_Thread{threadId}_二事件更新"
                    };
                    await EventBus.PublishAsync(threadUpdateCommand);

                    result.IsSuccess = true;
                    result.Message = "执行成功";
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = "执行失败";
                    result.Error = ex.ToString();
                }
                results.Add(result);
            }));
        }

        // 等待所有任务完成
        await Task.WhenAll(tasks);

        // 统计结果
        var summary = new
        {
            TotalThreads = threadCount,
            SuccessCount = results.Count(r => r.IsSuccess),
            FailCount = results.Count(r => !r.IsSuccess),
            SuccessRate = $"{Math.Round((double)results.Count(r => r.IsSuccess) / threadCount * 100, 2)}%",
            ExecuteTime = DateTime.Now,
            Details = results.OrderBy(r => r.ThreadId).ToList()
        };
        if (summary.SuccessCount == results.Count)
            return AdminResult.Success(summary);
        else
            return AdminResult.Fail($"部分线程执行失败", -1, summary);
    }

    [OpenApiTag("TestContent", Description = "多线程，多事件，多个事件同一个事务")]
    [RoutePattern(pattern: "createByMorenThreadMoreEventWithTran", true)]
    public async Task<AdminResult> CreateByMorenThreadMoreEventWithTranAsync([FromBody] TestContentCreateCommand command)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        int threadCount = 10;
        var tasks = new List<Task>();
        var results = new ConcurrentBag<ThreadResult>();
        // 创建多个任务并发执行
        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i; // 避免闭包问题
            tasks.Add(Task.Run(async () =>
            {
                var result = new ThreadResult { ThreadId = threadId };
                var dbResult = await SqlSugarClient.AsTenant().UseTranAsync(async () =>
                {
                    // 创建新的命令实例
                    var threadCommand = new TestContentCreateCommand
                    {
                        Name = $"{command.Name}_TrackId{traceId}_Thread{threadId}",
                        Remark = $"{command.Remark}_Thread{threadId}"
                    };
                    // 事件发布调用
                    // 无中间件  47.13 s   43.74 s  45.13 s
                    // 参数验证中间件  46.73 s  47.39 s  47.42 s
                    // 参数验证加单元事务中间件   165.67  148.66 s   148.52 s
                    await EventBus.PublishAsync(threadCommand);

                    // 服务直接调用   38 s   31.81 s   31.55 s
                    // await GetRequiredService<TestContentCommandHandler>().CreateHandleAsync(threadCommand);

                    var threadUpdateCommand = new TestContentUpdateCommand
                    {
                        Name = $"{command.Name}_TrackId{traceId}_Thread{threadId}",
                        Remark = $"{command.Remark}_Thread{threadId}_二事件更新"
                    };
                    await EventBus.PublishAsync(threadUpdateCommand);

                    result.IsSuccess = true;
                    result.Message = "执行成功";
                });
                if (dbResult.IsSuccess)
                {
                    result.IsSuccess = true;
                    result.Message = "执行成功";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "执行失败";
                    result.Error = dbResult.ErrorMessage;
                }
                results.Add(result);
            }));
        }

        // 等待所有任务完成
        await Task.WhenAll(tasks);

        // 统计结果
        var summary = new
        {
            TotalThreads = threadCount,
            SuccessCount = results.Count(r => r.IsSuccess),
            FailCount = results.Count(r => !r.IsSuccess),
            SuccessRate = $"{Math.Round((double)results.Count(r => r.IsSuccess) / threadCount * 100, 2)}%",
            ExecuteTime = DateTime.Now,
            Details = results.OrderBy(r => r.ThreadId).ToList()
        };
        if (summary.SuccessCount == results.Count)
            return AdminResult.Success(summary);
        else
            return AdminResult.Fail($"部分线程执行失败", -1, summary);
    }

    [OpenApiTag("TestContent", Description = "单线程,单事件多数据库,事件事务一致性")]
    [RoutePattern(pattern: "createByOneThreadOneEventWithTran", true)]
    public async Task<AdminResult> CreateByOneThreadOneEventWithTranAsync([FromBody] TestContentWithTranCreateCommand command)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        int threadCount = 1;
        var tasks = new List<Task>();
        var results = new ConcurrentBag<ThreadResult>();
        // 创建多个任务并发执行
        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i; // 避免闭包问题
            tasks.Add(Task.Run(async () =>
            {
                var result = new ThreadResult { ThreadId = threadId };
                try
                {
                    // 创建新的命令实例
                    var threadCommand = new TestContentWithTranCreateCommand
                    {
                        Name = $"{command.Name}_TrackId{traceId}_Thread{threadId}",
                        Remark = $"{command.Remark}_Thread{threadId}"
                    };
                    // 事件发布调用
                    // 无中间件  47.13 s   43.74 s  45.13 s
                    // 参数验证中间件  46.73 s  47.39 s  47.42 s
                    // 参数验证加单元事务中间件   165.67  148.66 s   148.52 s
                    await EventBus.PublishAsync(threadCommand);

                    result.IsSuccess = true;
                    result.Message = "执行成功";
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = "执行失败";
                    result.Error = ex.Message;
                }
                results.Add(result);
            }));
        }

        // 等待所有任务完成
        await Task.WhenAll(tasks);

        // 统计结果
        var summary = new
        {
            TotalThreads = threadCount,
            SuccessCount = results.Count(r => r.IsSuccess),
            FailCount = results.Count(r => !r.IsSuccess),
            SuccessRate = $"{Math.Round((double)results.Count(r => r.IsSuccess) / threadCount * 100, 2)}%",
            ExecuteTime = DateTime.Now,
            Details = results.OrderBy(r => r.ThreadId).ToList()
        };
        if (summary.SuccessCount == results.Count)
            return AdminResult.Success(summary);
        else
            return AdminResult.Fail($"部分线程执行失败", -1, summary);
    }

    // 事务不完整，多线程1个事件使用多个数据库事务处理
    // 只有1个事务回滚，原因是这里多个线程用的是同一个仓储对象，仓储对象上下文只会在构造函数赋值，导致仓储执行后面事务回滚都用的第一个事务的上下文
    // 改为  ITestContentRepository testContentRepository1 = new TestContentRepository();
    //       ITestContentLogRepository testContentLogRepository1 = new TestContentLogRepository
    // 这种形式就每个事务正常回滚         每个请求11s到12s
    [OpenApiTag("TestContent", Description = "多线程,单事件多数据库,事件事务一致性")]
    [RoutePattern(pattern: "createByMoreThreadOneEventWithTran", true)]
    public async Task<AdminResult> CreateByMoreThreadOneEventWithTranAsync([FromBody] TestContentWithTranCreateCommand command)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        int threadCount = 10;
        var tasks = new List<Task>();
        var results = new ConcurrentBag<ThreadResult>();
        // 创建多个任务并发执行
        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i; // 避免闭包问题
            tasks.Add(Task.Run(async () =>
            {
                var result = new ThreadResult { ThreadId = threadId };
                try
                {
                    // 创建新的命令实例
                    var threadCommand = new TestContentWithTranCreateCommand
                    {
                        Name = $"{command.Name}_TrackId{traceId}_Thread{threadId}",
                        Remark = $"{command.Remark}_Thread{threadId}"
                    };
                    // 事件发布调用
                    // 无中间件  47.13 s   43.74 s  45.13 s
                    // 参数验证中间件  46.73 s  47.39 s  47.42 s
                    // 参数验证加单元事务中间件   165.67  148.66 s   148.52 s
                    await EventBus.PublishAsync(threadCommand);

                    result.IsSuccess = true;
                    result.Message = "执行成功";
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = "执行失败";
                    result.Error = ex.Message;
                }
                results.Add(result);
            }));
        }

        // 等待所有任务完成
        await Task.WhenAll(tasks);

        // 统计结果
        var summary = new
        {
            TotalThreads = threadCount,
            SuccessCount = results.Count(r => r.IsSuccess),
            FailCount = results.Count(r => !r.IsSuccess),
            SuccessRate = $"{Math.Round((double)results.Count(r => r.IsSuccess) / threadCount * 100, 2)}%",
            ExecuteTime = DateTime.Now,
            Details = results.OrderBy(r => r.ThreadId).ToList()
        };
        if (summary.SuccessCount == results.Count)
            return AdminResult.Success(summary);
        else
            return AdminResult.Fail($"部分线程执行失败", -1, summary);
    }

    // 事务完整，嵌套事务已在中间件处理
    // 只有1个事务回滚，原因是这里多个线程用的是同一个仓储对象，仓储对象上下文只会在构造函数赋值，导致仓储执行后面事务回滚都用的第一个事务的上下文
    // 改为  ITestContentRepository testContentRepository1 = new TestContentRepository();
    //       ITestContentLogRepository testContentLogRepository1 = new TestContentLogRepository
    [OpenApiTag("TestContent", Description = "多线程,多事件多数据库,事件事务一致性")]
    [RoutePattern(pattern: "createByMorenThreadMoreEventOtherDBWithTran", true)]
    public async Task<AdminResult> CreateByMorenThreadMoreEventOtherDBWithTranAsync([FromBody] TestContentCreateCommand command)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        int threadCount = 10;
        var tasks = new List<Task>();
        var results = new ConcurrentBag<ThreadResult>();
        // 创建多个任务并发执行
        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i; // 避免闭包问题
            tasks.Add(Task.Run(async () =>
            {
                var result = new ThreadResult { ThreadId = threadId };
                try
                {
                    await SqlSugarClient.AsTenant().BeginTranAsync();

                    // 创建新的命令实例
                    var threadCommand = new TestContentCreateCommand
                    {
                        Name = $"{command.Name}_TrackId{traceId}_Thread{threadId}",
                        Remark = $"{command.Remark}_Thread{threadId}"
                    };
                    // 事件发布调用
                    // 无中间件  47.13 s   43.74 s  45.13 s
                    // 参数验证中间件  46.73 s  47.39 s  47.42 s
                    // 参数验证加单元事务中间件   165.67  148.66 s   148.52 s
                    await EventBus.PublishAsync(threadCommand);

                    // 服务直接调用   38 s   31.81 s   31.55 s
                    // await GetRequiredService<TestContentCommandHandler>().CreateHandleAsync(threadCommand);

                    var threadCommand2 = new TestContentLogCreateCommand
                    {
                        Name = $"{command.Name}_TrackId{traceId}_Thread{threadId}",
                        Remark = $"{command.Remark}_Thread{threadId}_log"
                    };
                    await EventBus.PublishAsync(threadCommand2);

                    result.IsSuccess = true;
                    result.Message = "执行成功";

                    await SqlSugarClient.AsTenant().CommitTranAsync();
                }
                catch (Exception ex)
                {
                    await SqlSugarClient.AsTenant().RollbackTranAsync();
                    result.IsSuccess = false;
                    result.Message = "执行失败";
                    result.Error = ex.Message;
                }
                results.Add(result);
            }));
        }

        // 等待所有任务完成
        await Task.WhenAll(tasks);

        // 统计结果
        var summary = new
        {
            TotalThreads = threadCount,
            SuccessCount = results.Count(r => r.IsSuccess),
            FailCount = results.Count(r => !r.IsSuccess),
            SuccessRate = $"{Math.Round((double)results.Count(r => r.IsSuccess) / threadCount * 100, 2)}%",
            ExecuteTime = DateTime.Now,
            Details = results.OrderBy(r => r.ThreadId).ToList()
        };
        if (summary.SuccessCount == results.Count)
            return AdminResult.Success(summary);
        else
            return AdminResult.Fail($"部分线程执行失败", -1, summary);
    }

    class ThreadResult
    {
        public int ThreadId { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime ExecuteTime { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }

    #endregion



    [OpenApiTag("TestContent", Description = "获取分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] TestContentPageQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("TestContent", Description = "获取列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] TestContentListQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }


    [OpenApiTag("TestContent", Description = "更新")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] TestContentUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("TestContent", Description = "删除")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new TestContentDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("TestContent", Description = "批量删除")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new TestContentDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}