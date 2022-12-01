using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UTB.EShop.Infrastructure.Entities.Configurations;

public sealed class ImageFileEntityConfiguration : IEntityTypeConfiguration<ImageFileEntity>
{
    public void Configure(EntityTypeBuilder<ImageFileEntity> builder)
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null && !directory.GetFiles("*.sln").Any()) directory = directory.Parent;
        var storagePath = Path.Combine(directory.FullName, "UTB.EShop.DistributedServices.WebAPI", "Development", "Uploads");

        builder.HasData(new List<ImageFileEntity>
        {
            new()
            {
                Id = 1,
                Path = Path.Combine(storagePath, "carousel_image_africa_giraffe.jpg")
            },
            new()
            {
                Id = 2,
                Path = Path.Combine(storagePath, "carousel_image_skyscraper.jpg")
            },
            new()
            {
                Id = 3,
                Path= Path.Combine(storagePath, "carousel_image_waterfall.jpg")
            }
        });
    }
}