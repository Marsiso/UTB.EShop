namespace UTB.EShop.Application.Paging;

public sealed class CarouselItemParameters : RequestParameters
{
    public DateTime MinDateTimeCreated { get; set; }
    public DateTime MaxDateTimeCreated { get; set; } = DateTime.Now;
    
    public bool IsValidDateRange => MaxDateTimeCreated > MinDateTimeCreated;
}