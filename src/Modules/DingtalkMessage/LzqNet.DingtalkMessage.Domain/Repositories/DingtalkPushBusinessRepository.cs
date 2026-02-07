using LzqNet.DingtalkMessage.Domain.Entities;
using LzqNet.DingtalkMessage.Domain.IRepositories;
using LzqNet.Extensions.SqlSugar.Repository;

namespace LzqNet.DingtalkMessage.Domain.Repositories;

public class DingtalkPushBusinessRepository()
    : SqlSugarRepository<DingtalkPushBusinessEntity>(), IDingtalkPushBusinessRepository
{

}