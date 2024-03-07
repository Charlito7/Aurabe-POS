using Core.Domain.Commons;
using Core.Domain.Entities;
using Core.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Infrastructure;

public class AppDbContext : IdentityDbContext<UserEntity, UserRoleEntity, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<ProductEntity> Products { get; set; }



#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        using (var transaction = Database.BeginTransaction())
        {
            try
            {

                var entries = ChangeTracker.Entries()
                    .Where(e => e.Entity is AuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

                foreach (var entry in entries)
                {
                    var entity = (AuditableEntity)entry.Entity;

                    if (entry.State == EntityState.Added)
                    {
                        entity.Created = DateTime.UtcNow;
                        entity.CreatedBy = "SetCreatedBy"; // Set the appropriate value or retrieve it from the current user, if applicable.
                        entity.IsDeleted = false;
                    }

                    entity.LastModified = DateTime.UtcNow;
                    entity.LastModifiedBy = "SetLastModifiedBy"; // Set the appropriate value or retrieve it from the current user, if applicable.
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserEntity>(entity => { entity.ToTable("Users"); });
        builder.Entity<UserRoleEntity>(entity => { entity.ToTable("Roles"); });
        builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
        builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });
        builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
        builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });
        builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
        builder.Entity<ProductEntity>(entity => { entity.ToTable("Products"); });
        builder.Entity<CategoryEntity>(entity => { entity.ToTable("Categories"); });

        // Configure entity properties, relationships, etc.
        /*modelBuilder.ApplyConfiguration(new SupplierEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());*/
        // Add configurations for other entities if needed
    }
}
