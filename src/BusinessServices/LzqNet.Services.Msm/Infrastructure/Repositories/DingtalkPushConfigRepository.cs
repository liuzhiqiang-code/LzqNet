using LzqNet.Extensions.SqlSugar.Repository;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Infrastructure.Repositories;

public class DingtalkPushConfigRepository()
    : SqlSugarRepository<DingtalkPushConfigEntity>(), IDingtalkPushConfigRepository
{

}