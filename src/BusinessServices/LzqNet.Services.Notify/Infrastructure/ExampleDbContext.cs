using LzqNet.Services.Notify.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LzqNet.Services.Notify.Infrastructure;

public class ExampleDbContext : MasaDbContext
{
    public ExampleDbContext(MasaDbContextOptions<ExampleDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        base.OnModelCreatingExecuting(modelBuilder);
        ConfigEntities(modelBuilder);
    }

    private static void ConfigEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SysConfigEntity>(entity =>
        {
            // 配置表名
            entity.ToTable("sys_config");
        });
    }
}