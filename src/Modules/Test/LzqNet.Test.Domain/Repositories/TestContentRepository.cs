using LzqNet.Extensions.SqlSugar.Repository;
using LzqNet.Test.Domain.Entities;
using LzqNet.Test.Domain.IRepositories;

namespace LzqNet.Test.Domain.Repositories;

public class TestContentRepository()
    : SqlSugarRepository<TestContentEntity>(), ITestContentRepository
{

}