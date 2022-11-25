using Microsoft.AspNetCore.Mvc;

namespace UTB.EShop.DistributedServices.WebAPI.Controllers;

[ApiController]
[Route("Carousel")]
public class CarouselController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok();
    }
}