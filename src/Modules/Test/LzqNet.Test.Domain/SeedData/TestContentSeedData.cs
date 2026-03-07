using LzqNet.Extensions.SqlSugar.Entities;
using LzqNet.Test.Domain.Entities;

namespace LzqNet.Test.Domain.SeedData;

public class TestContentSeedData : BaseSeedData<TestContentEntity>
{
    public override List<TestContentEntity> GetSeedData()
    {
        return new List<TestContentEntity>
        {
            new TestContentEntity
            {
                Id = 1,
                Name = "Test Content 1",
                Remark = "This is the first test content."
            },
            new TestContentEntity
            {
                Id = 2,
                Name = "Test Content 2",
                Remark = "This is the second test content."
            },
            new TestContentEntity
            {
                Id = 3,
                Name = "Test Content 3",
                Remark = "This is the third test content."
            }
        };
    }
}
