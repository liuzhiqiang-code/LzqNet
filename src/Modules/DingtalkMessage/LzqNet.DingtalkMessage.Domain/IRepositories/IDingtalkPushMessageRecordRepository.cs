using LzqNet.DingtalkMessage.Domain.Entities;
using LzqNet.Extensions.SqlSugar.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.DingtalkMessage.Domain.IRepositories;

public interface IDingtalkPushMessageRecordRepository : ISqlSugarRepository<DingtalkPushMessageRecordEntity>, ITransientDependency
{

}