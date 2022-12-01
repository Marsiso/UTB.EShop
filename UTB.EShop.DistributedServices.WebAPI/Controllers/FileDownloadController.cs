using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using UTB.EShop.Application.DataTransferObjects.Image;
using UTB.EShop.Application.Interfaces.Repositories;
using UTB.EShop.DistributedServices.WebAPI.Attributes;
using UTB.EShop.DistributedServices.WebAPI.Utility;
using UTB.EShop.Infrastructure.Entities;
using ILogger = Serilog.ILogger;

namespace UTB.EShop.DistributedServices.WebAPI.Controllers;

[Route("api/Carousel/{id:int}/[Controller]")]
[ApiController]
[ApiVersion("1.0")]
public class FileDownloadController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IRepository<CarouselItemEntity> _carouselItemRepository;
    private readonly IRepository<ImageFileEntity> _imageFileRepository;

    public FileDownloadController(ILogger logger, IRepository<CarouselItemEntity> carouselItemRepository, IRepository<ImageFileEntity> imageFileRepository)
    {
        _logger = logger;
        _carouselItemRepository = carouselItemRepository;
        _imageFileRepository = imageFileRepository;
    }

    [HttpGet(Name = "DownloadSingleImageFile")]
    [ServiceFilter(typeof(ValidateCarouselItemExistsAttribute))]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)] 
    [HttpCacheValidation(MustRevalidate = false)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownloadSingleImageFile(int id)
    {
        var carouselItemEntity = HttpContext.Items["carouselItem"] as CarouselItemEntity;
        var imageFileEntity = await _imageFileRepository.GetEntityAsync(carouselItemEntity!.ImageFileId);
        if (imageFileEntity is not null && System.IO.File.Exists(imageFileEntity.Path))
        {
            try
            {
                if (Path.GetExtension(imageFileEntity.Path).TryGetMimeTypeFromExtension(out var mimoType))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(imageFileEntity.Path);                    
                    _logger.Warning("Image file successfully created");
                    return File(bytes, mimoType!, Path.GetFileName(imageFileEntity.Path));
                }

                _logger.Warning("Extension type conversion failure. Extension {Extension}", 
                    Path.GetExtension(imageFileEntity.Path));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured on file download. Message: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        _logger.Warning("Carousel image with ID: {CarouselItemID} image file with ID: {ImageID} hasn't been found",
            id,
            carouselItemEntity.ImageFileId);
        
        return NotFound();
    }
}