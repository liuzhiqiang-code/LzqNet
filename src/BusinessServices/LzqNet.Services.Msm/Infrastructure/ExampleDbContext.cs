using LzqNet.Caller.Msm.Contracts.Menu;
using LzqNet.Services.Msm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
        modelBuilder.Entity<DeptEntity>().ToTable("msm_dept");
        modelBuilder.Entity<RoleEntity>().ToTable("msm_role");

        // 在DbContext中配置菜单实体
        modelBuilder.Entity<MenuEntity>(entity =>
        {
            var jsonOptions = new JsonSerializerOptions();
            // 配置Meta字段的JSON转换（带null检查）
            entity.Property(e => e.Meta)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, jsonOptions) : null,
                    v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<MenuMeta>(v, jsonOptions) : null
                )
                .HasMaxLength(int.MaxValue);  // 明确指定数据库类型

            // 枚举转字符串配置
            entity.Property(e => e.Type)
                  .HasConversion<string>()
                  .HasMaxLength(50);  // 添加长度限制

            // 表名映射
            entity.ToTable("msm_menu");

            // 建议添加的索引配置（根据实际查询需求）
            entity.HasIndex(e => e.Pid);
            entity.HasIndex(e => e.AuthCode).IsUnique();
        });
    }
}