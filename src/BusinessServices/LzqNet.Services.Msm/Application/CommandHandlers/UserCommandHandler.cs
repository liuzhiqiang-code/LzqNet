using Masa.Contrib.Dispatcher.Events;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using LzqNet.Caller.Msm.Contracts.User.Commands;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class UserCommandHandler(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    [EventHandler]
    public async Task CreateHandleAsync(UserCreateCommand command)
    {
        var entity = command.Map<UserEntity>();
        await _userRepository.AddAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(UserUpdateCommand command)
    {
        var entity = command.Map<UserEntity>();
        await _userRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(UserDeleteCommand command)
    {
        await _userRepository.RemoveAsync(a => command.Ids.Contains(a.Id));
    }
}