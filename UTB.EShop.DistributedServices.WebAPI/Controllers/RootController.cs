using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using UTB.EShop.Application.Hateos;

namespace UTB.EShop.DistributedServices.WebAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api")]
[ApiExplorerSettings(GroupName = "v1")]
//[ResponseCache(CacheProfileName = "120SecondsDuration")]
[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)] 
[HttpCacheValidation(MustRevalidate = false)]
public class RootController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;

    public RootController(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }
    
    [HttpGet(Name = "GetRoot")]
    public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
    {
        if(mediaType.Contains("application/vnd.apiroot"))
        {
            var list = new List<Link>
            {
                new Link
                { 
                    Href = _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new {}),
                    Rel = "self", 
                    Method = "GET"
                },
                new Link
                {
                    Href = _linkGenerator.GetUriByName(HttpContext, "GetAllCarouselItems", new {}),
                    Rel = "get_carousel_items",
                    Method = "GET"
                },
                new Link
                {
                    Href = _linkGenerator.GetUriByName(HttpContext, "GetAllCarouselItems", new {}),
                    Rel = "create_carousel_item",
                    Method = "POST"
                }
            };
            
            return Ok(list);
        }
        
        return NoContent();
    }
}