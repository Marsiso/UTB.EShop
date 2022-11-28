using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using UTB.EShop.Application.Interfaces.Entities;

namespace UTB.EShop.Infrastructure.Entities;

[Table("CarouselItems")]
public sealed class CarouselItemEntity : AuditableEntity, IDataEntity
{
    [Key]
    [Column("pk_carousel_item")]
    public  int Id { get; set; }

    [Column("image_alt")]
    [Required]
    [StringLength(50)]
    [DataType(DataType.Text)]
    public string ImageAlt { get; set; } = null!;

    [Column("image_caption_header")]
    [AllowNull]
    [StringLength(50)]
    [DataType(DataType.Text)]
    public string? ImageCaptionHeader { get; set; }
    
    [Column("image_caption_text")]
    [AllowNull]
    [StringLength(50)]
    [DataType(DataType.Text)]
    public string? ImageCaptionText { get; set; }

    [NotMapped]
    public ImageFileEntity Image { get; set; } = null!;
}