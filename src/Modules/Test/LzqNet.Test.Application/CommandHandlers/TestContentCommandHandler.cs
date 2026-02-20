using LzqNet.Test.Contracts.TestContent.Commands;
using LzqNet.Test.Domain.Entities;
using LzqNet.Test.Domain.IRepositories;
using LzqNet.Test.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Serilog;

namespace LzqNet.Test.Application.CommandHandlers;

public class TestContentCommandHandler(ITestContentRepository testContentRepository, ITestContentLogRepository testContentLogRepository)
{
    private readonly ITestContentRepository _testContentRepository = testContentRepository;
    private readonly ITestContentLogRepository _testContentLogRepository = testContentLogRepository;

    [EventHandler]
    public async Task CreateHandleAsync(TestContentCreateCommand command)
    {
        ITestContentRepository testContentRepository1 = new TestContentRepository();
        var entity = command.Map<TestContentEntity>();
        var random = new Random(Guid.NewGuid().GetHashCode());
        // 1.新增数据    1/100 概率失败
        if (random.Next(1, 100) != 1)
        {
            await testContentRepository1.InsertAsync(entity);
            Log.Information("随机新增成功，命令：{@Command}", command);
        }
        else {
            Log.Information("随机新增失败，命令：{@Command}", command);
            throw new UserFriendlyException("随机新增失败");
        }


        // 2.修改数据
        if (random.Next(1, 100) != 1)
        {
            entity.Remark += "_后续更新";
            await _testContentRepository.UpdateAsync(entity);
            Log.Information("随机更新成功，命令：{@Command}", command);
        }
        else
        {
            Log.Information("随机修改失败，命令：{@Command}", command);
            throw new UserFriendlyException("随机修改失败");
        }
    }

    [EventHandler]
    public async Task CreateWithTranHandleAsync(TestContentWithTranCreateCommand command)
    {
        ITestContentRepository testContentRepository1 = new TestContentRepository();
        ITestContentLogRepository testContentLogRepository1 = new TestContentLogRepository();
        var entity = command.Map<TestContentEntity>();
        var random = new Random(Guid.NewGuid().GetHashCode());
        // 1.新增数据    1/100 概率失败
        if (random.Next(1, 100) != 1)
        {
            await testContentRepository1.InsertAsync(entity);
            Log.Information("随机新增成功，命令：{@Command}", command);
        }
        else
        {
            Log.Information("随机新增失败，命令：{@Command}", command);
            throw new UserFriendlyException("随机新增失败");
        }


        // 2.第2个数据库操作
        if (random.Next(1, 100) == -1)//故意不成功，测第1个数据库操作是否回滚
        {
            var entity2 = command.Map<TestContentLogEntity>();
            entity.Remark += "_log";
            await testContentLogRepository1.InsertAsync(entity2);
            Log.Information("随机新增log成功，命令：{@Command}", command);
        }
        else
        {
            Log.Information("随机新增log失败，命令：{@Command}", command);
            throw new UserFriendlyException("随机新增log失败");
        }
    }


    [EventHandler]
    public async Task UpdateHandleAsync(TestContentUpdateCommand command)
    {
        ITestContentRepository testContentRepository1 = new TestContentRepository();
        ITestContentLogRepository testContentLogRepository1 = new TestContentLogRepository();
        var random = new Random(Guid.NewGuid().GetHashCode());
        // 1.新增数据    1/100 概率失败
        if (random.Next(1, 100) != 1)
        {
            var entity = await _testContentRepository.GetFirstAsync(a => a.Name.Equals(command.Name));
            if (entity == null)
            {
                Log.Information("未找到新增事件新增的数据，命令：{@Command}", command);
                throw new UserFriendlyException("未找到新增事件新增的数据");
            }
            else
            {
                entity.Remark += "_更新事件执行成功";
                await _testContentRepository.UpdateAsync(entity);
                Log.Information("命令随机更新成功，命令：{@Command}", command);
            }
        }
        else
        {
            Log.Information("命令随机更新失败，命令：{@Command}", command);
            throw new UserFriendlyException("命令随机更新失败");
        }
    }

    [EventHandler]
    public async Task DeleteHandleAsync(TestContentDeleteCommand command)
    {
        await _testContentRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}