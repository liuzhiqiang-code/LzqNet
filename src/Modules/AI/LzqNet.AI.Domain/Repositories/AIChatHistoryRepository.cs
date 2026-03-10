using LzqNet.AI.Domain.Entities;
using LzqNet.AI.Domain.IRepositories;
using LzqNet.Extensions.SqlSugar.Repository;

namespace LzqNet.AI.Domain.Repositories;

public class AIChatHistoryRepository()
    : SqlSugarRepository<AIChatHistoryEntity>(), IAIChatHistoryRepository
{

}