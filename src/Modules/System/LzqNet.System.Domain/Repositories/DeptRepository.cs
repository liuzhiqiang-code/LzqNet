using LzqNet.Extensions.SqlSugar.Repository;
using LzqNet.System.Domain.Entities;
using LzqNet.System.Domain.IRepositories;

namespace LzqNet.System.Domain.Repositories;

public class DeptRepository()
    : SqlSugarRepository<DeptEntity>(), IDeptRepository
{

}