using LzqNet.Caller.Common.Contracts;
using LzqNet.Caller.Msm.Contracts.Menu;
using LzqNet.Services.Msm.Domain.Entities;
using Masa.BuildingBlocks.Ddd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

    ///   TODO  这个拦截器没生效
    protected override void OnBeforeSaveChanges()
    {
        var now = DateTime.Now;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is BaseFullEntity entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        SetCreationFields(entity, now);
                        break;

                    case EntityState.Modified:
                        SetModificationFields(entity, now);
                        break;

                    case EntityState.Deleted:
                        HandleSoftDelete(entry, entity, now);
                        break;
                }
            }
        }
        base.OnBeforeSaveChanges();
    }
    private void SetCreationFields(BaseFullEntity entity, DateTime now)
    {
        // entity.Creator = GetCurrentUserId(); // 根据实际情况获取当前用户
        entity.CreationTime = now;
        // entity.Modifier = GetCurrentUserId();
        entity.ModificationTime = now;
        entity.IsDeleted = false;
    }

    private void SetModificationFields(BaseFullEntity entity, DateTime now)
    {
        // entity.Modifier = GetCurrentUserId();
        entity.ModificationTime = now;
    }

    private void HandleSoftDelete(EntityEntry entry, BaseFullEntity entity, DateTime now)
    {
        // 实现软删除
        SetModificationFields(entity, now);
        entity.IsDeleted = true;
        entry.State = EntityState.Modified; // 重要：将删除改为修改
    }

    private static void ConfigEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SysConfigEntity>();
        modelBuilder.Entity<DeptEntity>();
        modelBuilder.Entity<RoleEntity>();
        modelBuilder.Entity<RoleAuthEntity>();
        modelBuilder.Entity<UserEntity>();

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

            // 建议添加的索引配置（根据实际查询需求）
            entity.HasIndex(e => e.Pid);
            entity.HasIndex(e => e.AuthCode).IsUnique();
        });
    }
}