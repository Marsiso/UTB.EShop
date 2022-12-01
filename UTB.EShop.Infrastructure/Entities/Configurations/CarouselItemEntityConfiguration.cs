using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UTB.EShop.Infrastructure.Entities.Configurations;

public sealed class CarouselItemEntityConfiguration : IEntityTypeConfiguration<CarouselItemEntity>
{
    public void Configure(EntityTypeBuilder<CarouselItemEntity> builder)
    {
        builder.HasData(new List<CarouselItemEntity>
        {
            new()
            {
                Id = 1,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                CreatedBy = "Marek Olsak",
                ModifiedBy = "Marek Olsak",
                ImageAlt = "image",
                ImageCaptionHeader = "Quote",
                ImageCaptionText = "Even Sun have sunny days ...",
                ImageFileId = 1
            },
            new()
            {
                Id = 2,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                CreatedBy = "Alexandr Cizek",
                ModifiedBy = "Marek Olsak",
                ImageAlt = "image",
                ImageCaptionHeader = "Quote",
                ImageCaptionText = "Water is wet ...",
                ImageFileId = 2
            },
            new()
            {
                Id = 3,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                CreatedBy = "Marek Olsak",
                ModifiedBy = "Alexandr Cizek",
                ImageAlt = "image",
                ImageCaptionHeader = "Quote",
                ImageCaptionText = "Gold is shiny ...",
                ImageFileId = 3
            }
        });
    }
}