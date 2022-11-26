using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UTB.EShop.Infrastructure.Entities.Configurations;

public sealed class ImageFileEntityConfiguration : IEntityTypeConfiguration<ImageFileEntity>
{
    public void Configure(EntityTypeBuilder<ImageFileEntity> builder)
    {
        builder.HasData(new List<ImageFileEntity>
        {
            new()
            {
                Id = 1,
                FileName = "qvqcc2pc.zjd",
                DisplayName = "carousel-image-0",
                CarouselItemId = 1
            },
            new()
            {
                Id = 2,
                FileName = "2ubsxume.w4z",
                DisplayName = "carousel-image-1",
                CarouselItemId = 2
            },
            new()
            {
                Id = 3,
                FileName = "nufctnyg.bf0",
                DisplayName = "carousel-image-2",
                CarouselItemId = 3
            }
        });
    }
}