namespace UTB.EShop.Application.DataTransferObjects.Carousel;

public sealed class CarouselItemDto
{
    public  int Id { get; set; }
    
    public string ImageAlt { get; set; } = null!;
    
    public string? ImageCaptionHeader { get; set; }
    
    public string? ImageCaptionText { get; set; }
}