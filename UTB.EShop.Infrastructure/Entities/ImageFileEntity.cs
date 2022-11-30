﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization.Formatters;
using UTB.EShop.Application.Interfaces.Entities;

namespace UTB.EShop.Infrastructure.Entities;

[Table("ImageFiles")]
public sealed class ImageFileEntity : IDataEntity
{
    [Key]
    [Column("pk_image_file")]
    public int Id { get; set; }

    [Column("file_path")]
    [Required]
    [DataType(DataType.Text)]
    public string Path { get; set; } = null!;

    [Column("fk_carousel_item")]
    [ForeignKey(nameof(CarouselItemEntity))]
    public  int CarouselItemId { get; set; }

    [NotMapped] public CarouselItemEntity CarouselItemEntity { get; set; } = null!;
}