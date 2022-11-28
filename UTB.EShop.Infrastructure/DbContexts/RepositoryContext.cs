using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UTB.EShop.Infrastructure.Entities;
using UTB.EShop.Infrastructure.Entities.Configurations;
using UTB.EShop.Infrastructure.Identity;
using UTB.EShop.Infrastructure.Identity.Configurations;

namespace UTB.EShop.Infrastructure.DbContexts;

public sealed class RepositoryContext : IdentityDbContext<User>
{
    public DbSet<CarouselItemEntity> CarouselItemEntities { get; set; } = null!;
    public DbSet<ImageFileEntity> ImageFileEntities { get; set; } = null!;

    public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new CarouselItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ImageFileEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
    }
}