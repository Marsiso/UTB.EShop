using System.Dynamic;
using Microsoft.Net.Http.Headers;
using UTB.EShop.Application.DataTransferObjects.Carousel;
using UTB.EShop.Application.Hateos;
using UTB.EShop.Application.Interfaces.Models;

namespace UTB.EShop.DistributedServices.WebAPI.Utility;

public sealed class CarouselItemLinks
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IDataShaper<CarouselItemDto> _dataShaper;
    
    public CarouselItemLinks(LinkGenerator linkGenerator, IDataShaper<CarouselItemDto> 
        dataShaper)
    {
        _linkGenerator = linkGenerator;
        _dataShaper = dataShaper;
    }

    public LinkResponse TryGenerateLinks(IEnumerable<CarouselItemDto> carouselItemsDto, string? fields, HttpContext httpContext)
    {
        var enumeration = carouselItemsDto as CarouselItemDto[] ?? carouselItemsDto.ToArray();
        var shapedCarouselItems = ShapeData(enumeration, fields);
        if (ShouldGenerateLinks(httpContext))
            return ReturnLinkedCarouselItems(enumeration, fields, httpContext, shapedCarouselItems);
        return ReturnShapedCarouselItems(shapedCarouselItems);
    }

    private List<ExpandoObject> ShapeData(IEnumerable<CarouselItemDto> carouselItemDtos, string? fields) =>
            _dataShaper.ShapeData(carouselItemDtos, fields)
                .Select(e => e.Entity)
                .ToList();
    
    private bool ShouldGenerateLinks(HttpContext httpContext)
    {
        var mediaType = (MediaTypeHeaderValue?)httpContext.Items["AcceptHeaderMediaType"];
        if (mediaType is null) return false;
        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }
    
    private LinkResponse ReturnShapedCarouselItems(List<ExpandoObject> shapedCarouselItems) => new
        LinkResponse { ShapedEntities = shapedCarouselItems };

    private LinkResponse ReturnLinkedCarouselItems(IEnumerable<CarouselItemDto> carouselItemsDto, 
        string? fields, HttpContext httpContext, List<ExpandoObject> shapedCarouselItems)
    {
        var carouselItemsDtoList = carouselItemsDto.ToList();
        for (var index = 0; index < carouselItemsDtoList.Count(); index++)
        {
            var carouselItemLinks = CreateLinksForCarouselItem(httpContext, carouselItemsDtoList[index].Id, fields);
            _ = shapedCarouselItems[index].TryAdd("Links", carouselItemLinks);
        }
        
        var carouselItemsCollection = new LinkCollectionWrapper<ExpandoObject>(shapedCarouselItems);
        var linkedCarouselItems = CreateLinksForCarouselItems(httpContext, carouselItemsCollection);
        return new LinkResponse { HasLinks = true, LinkedEntities = linkedCarouselItems };
    }

    private List<Link> CreateLinksForCarouselItem(HttpContext httpContext, int id, string? fields = "")
    {
        fields ??= "";
        var links = new List<Link>
        {
            new Link(
                _linkGenerator.GetUriByAction(httpContext, "GetCarouselItemById", "Carousel", values: new { id, fields }), 
                    "self", 
                    "GET"),
                        
            new Link(
                _linkGenerator.GetUriByAction(httpContext, "DeleteCarouselItem", "Carousel", values: new { id }),
                "delete_carousel_item",
                "DELETE"),
                
            new Link(
                _linkGenerator.GetUriByAction(httpContext, "UpdateCarouselItem", "Carousel", values: new { id }),
                "update_carousel_item",
                "PUT"),
                
            new Link(
                _linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateCarouselItem", "Carousel", values: new { id }),
                "partially_update_carousel_item",
                "PATCH")
        };
        
        return links;
    }
    private LinkCollectionWrapper<ExpandoObject> CreateLinksForCarouselItems(HttpContext httpContext, 
        LinkCollectionWrapper<ExpandoObject> carouselItemsWrapper)
    {
        carouselItemsWrapper.Links.Add(new Link(
            _linkGenerator.GetUriByAction(httpContext,"GetAllCarouselItems","Carousel",values: new { })!,
            "self",
            "GET"));
        
        return carouselItemsWrapper;
    }
}