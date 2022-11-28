using UTB.EShop.Application.Interfaces.Entities;

namespace UTB.EShop.Application.DataTransferObjects.Carousel;

public sealed class CarouselItemDto : IDataEntity
{
    public  int Id { get; set; }
    
    public string ImageAlt { get; set; } = null!;
    
    public string? ImageCaptionHeader { get; set; }
    
    public string? ImageCaptionText { get; set; }
    
    /// <summary>
    /// The date the entity was created
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// The date the entity was last modified
    /// </summary>

    public DateTime DateModified { get; set; }

    /// <summary>
    /// The user who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The user who last modified the entity
    /// </summary>
    public string? ModifiedBy { get; set; }
}