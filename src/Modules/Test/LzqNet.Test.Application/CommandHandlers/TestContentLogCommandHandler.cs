using LzqNet.Test.Contracts.TestContentLog.Commands;
using LzqNet.Test.Domain.Entities;
using LzqNet.Test.Domain.IRepositories;
using LzqNet.Test.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Serilog;

namespace LzqNet.Test.Application.CommandHandlers;

public class TestContentLogCommandHandler(ITestContentLogRepository testContentLogRepository)
{
    private readonly ITestContentLogRepository _testContentLogRepository = testContentLogRepository;

    [EventHandler]
    public async Task CreateHandleAsync(TestContentLogCreateCommand command)
    {
        ITestContentLogRepository testContentLogRepository1 = new TestContentLogRepository();
        var entity = command.Map<TestContentLogEntity>();
        var random = new Random(Guid.NewGuid().GetHashCode());
        // 1.新增数据    1/100 概率失败
        if (random.Next(1, 100) == -1) //故意失败，测第一个库的事件是否会回滚
        {
            await testContentLogRepository1.InsertAsync(entity);
            Log.Information("随机新增成功，命令：{@Command}", command);
        }
        else {
            Log.Information("随机新增失败，命令：{@Command}", command);
            throw new UserFriendlyException("随机新增失败");
        }
    }

    [EventHandler]
    public async Task UpdateHandleAsync(TestContentLogUpdateCommand command)
    {
        var random = new Random(Guid.NewGuid().GetHashCode());
        // 1.新增数据    1/100 概率失败
        if (random.Next(1, 100) != 1)
        {
            var entity = await _testContentLogRepository.GetFirstAsync(a => a.Name.Equals(command.Name));
            if (entity == null)
            {
                Log.Information("未找到新增事件新增的数据，命令：{@Command}", command);
                throw new UserFriendlyException("未找到新增事件新增的数据");
            }
            else
            {
                entity.Remark += "_更新事件执行成功";
                await _testContentLogRepository.UpdateAsync(entity);
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
    public async Task DeleteHandleAsync(TestContentLogDeleteCommand command)
    {
        await _testContentLogRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}