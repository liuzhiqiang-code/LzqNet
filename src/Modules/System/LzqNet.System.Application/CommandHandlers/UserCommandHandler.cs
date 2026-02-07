using Masa.Contrib.Dispatcher.Events;
using LzqNet.System.Contracts.User.Commands;
using LzqNet.System.Domain.Entities;
using LzqNet.System.Domain.IRepositories;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Application.CommandHandlers;

public class UserCommandHandler(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    [EventHandler]
    public async Task CreateHandleAsync(UserCreateCommand command)
    {
        var entity = command.Map<UserEntity>();
        await _userRepository.InsertAsync(entity);
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
        await _userRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}