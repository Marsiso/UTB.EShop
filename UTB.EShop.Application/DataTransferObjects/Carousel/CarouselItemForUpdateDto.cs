using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UTB.EShop.Application.DataTransferObjects.Carousel;

public sealed class CarouselItemForUpdateDto
{
    [StringLength(50)]
    [DataType(DataType.Text)]
    [JsonPropertyName("imageAlt")]
    public string? ImageAlt { get; set; }

    [StringLength(50)]
    [DataType(DataType.Text)]
    [JsonPropertyName("imageCaptionHeader")]
    public string? ImageCaptionHeader { get; set; }
    
    [StringLength(50)]
    [DataType(DataType.Text)]
    [JsonPropertyName("imageCaptionText")]
    public string? ImageCaptionText { get; set; }
}