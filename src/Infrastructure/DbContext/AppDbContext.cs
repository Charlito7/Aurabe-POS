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
    public DbSet<GetSellerSalesTotalPriceAndQuantityToday> GetSellerSalesTotalPriceAndQuantityTodays { get; set; }
    public DbSet<SellerDailyResumeEntity> SellerDailyResumes { get; set; }



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
        builder.Entity<UserEntity>(entity => { entity.ToTable("users"); });
        builder.Entity<UserRoleEntity>(entity => { entity.ToTable("roles"); });
        builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("userclaims"); });
        builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("userlogins"); });
        builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("roleclaims"); });
        builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("usertokens"); });
        builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("userroles"); });
        builder.Entity<ProductEntity>(entity => { entity.ToTable("products"); });
        builder.Entity<CategoryEntity>(entity => { entity.ToTable("categories"); });
        builder.Entity<SalesMetadataEntity>(entity => { entity.ToTable("salesmetadata"); });
        builder.Entity<ProductSalesEntity>(entity => { entity.ToTable("productsales"); });
        builder.Entity<GetProductSuggestionsResponse>().HasNoKey();
        builder.Entity<SalesMetadataAndProductResponse>().HasNoKey(); 
        builder.Entity<GetAllSalesMetadata>().HasNoKey();
        builder.Entity<SellerDailyResumeEntity>().HasNoKey();
        builder.Entity<GetSellerSalesTotalPriceAndQuantityToday>().HasNoKey();

    }
}
