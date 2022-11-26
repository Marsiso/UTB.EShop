using Microsoft.EntityFrameworkCore;
using UTB.EShop.Infrastructure.Entities;
using UTB.EShop.Infrastructure.Entities.Configurations;

namespace UTB.EShop.Infrastructure.Repositories;

public sealed class RepositoryContext : DbContext
{
    public DbSet<CarouselItemEntity> CarouselItemEntities { get; set; } = null!;
    public DbSet<ImageFileEntity> ImageFileEntities { get; set; } = null!;

    public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CarouselItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ImageFileEntityConfiguration());
    }
}