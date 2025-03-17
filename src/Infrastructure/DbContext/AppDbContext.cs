using Core.Application.Model.Response;
using Core.Domain.Commons;
using Core.Domain.Entities;
using Core.Domain.Entity;
using Core.Domain.Procedures;
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
    public DbSet<ProductSalesEntity> ProductSales { get; set; }
    public DbSet<SalesMetadataEntity> SalesMetadata { get; set; }
    public DbSet<GetProductSuggestionsResponse> ProductSuggestionsResponses { get; set; }
    public DbSet<SalesMetadataAndProductResponse> SalesMetadataAndProductResponses { get; set; }
    public DbSet<GetAllSalesMetadata> GetAllSalesMetadata { get; set; }



#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is AuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (AuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.Created = DateTime.UtcNow;
                entity.IsDeleted = false;
            }

            entity.LastModified = DateTime.UtcNow;
            entity.LastModifiedBy = "SetLastModifiedBy";
        }

        // No manual transaction; let EF handle it
        return await base.SaveChangesAsync(cancellationToken);
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
        builder.Entity<SalesMetadataEntity>(entity => { entity.ToTable("SalesMetadata"); });
        builder.Entity<ProductSalesEntity>(entity => { entity.ToTable("ProductSales"); });
        builder.Entity<GetProductSuggestionsResponse>().HasNoKey();
        builder.Entity<SalesMetadataAndProductResponse>().HasNoKey(); 
        builder.Entity<GetAllSalesMetadata>().HasNoKey();

    }
}
