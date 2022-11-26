using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UTB.EShop.Application.DataTransferObjects.Carousel;

public sealed class CarouselItemForCreationDto
{
    [Required]
    [StringLength(50)]
    [DataType(DataType.Text)]
    public string ImageAlt { get; set; } = null!;

    [StringLength(50)]
    [DataType(DataType.Text)]
    public string? ImageCaptionHeader { get; set; }
    
    [StringLength(50)]
    [DataType(DataType.Text)]
    public string? ImageCaptionText { get; set; }
}