using LzqNet.Services.Msm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LzqNet.Services.Msm.Infrastructure;

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
        modelBuilder.Entity<SysConfigEntity>().ToTable("sys_config");
        modelBuilder.Entity<DeptEntity>().ToTable("msm_Dept");
    }
}