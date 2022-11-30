using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UTB.EShop.Application.DataTransferObjects.Image;
using UTB.EShop.Application.Interfaces.Repositories;
using UTB.EShop.Application.Paging;
using UTB.EShop.DistributedServices.WebAPI.Attributes;
using UTB.EShop.DistributedServices.WebAPI.Utility;
using UTB.EShop.Infrastructure.Entities;
using UTB.EShop.Infrastructure.Models;
using Constants = UTB.EShop.Application.Constants.Constants;
using ILogger = Serilog.ILogger;

namespace UTB.EShop.DistributedServices.WebAPI.Controllers;

[Route("api/Carousel/{id:int}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class FileUploadController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IRepository<CarouselItemEntity> _carouselRepository;
    private readonly IRepository<ImageFileEntity> _imageFileRepository;
    private enum ErrorCodes
    {
        FileZeroLengthError = 1,
        FileLengthOutOfBoundsError = 2,
        FileOnUploadError = 3,
        FileUploadLimitExceededError = 4,
        FileMimoTypeToExtensionError = 5
    }

    public FileUploadController(ILogger logger, IHostEnvironment hostEnvironment, IRepository<CarouselItemEntity> carouselRepository, IRepository<ImageFileEntity> imageFileRepository)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _carouselRepository = carouselRepository;
        _imageFileRepository = imageFileRepository;
    }

    [HttpPost(Name = "UploadSingleImage")]
    [ServiceFilter(typeof(ValidationFilterAttribute), Order = 1)]
    [ServiceFilter(typeof(ValidateCarouselItemExistsAttribute), Order = 2)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadSingleImage(int id, [FromForm]SingleImageFileForCreationDto? file)
    {
        // Validation attribute validates our model
        Debug.Assert(file != null, "image.File != null");
        Debug.Assert(file.File != null, "image.File != null");

        var responseModelDto = new ResponseModelDto();
        responseModelDto.IsResponse = true;
        
        var untrustedFileName = file.FileName;
        var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);
        var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
        responseModelDto.IsSuccess = false;
        
        // File length validation
        if (file.File.Length is >= Constants.MaxImageFileUploadSize or <= Constants.MinImageFileUploadSize)
        {
            _logger.Warning("File {FileName}'s MIMO type doesn't match any file extensions. (Err: {ErrorCode}).",
                trustedFileNameForDisplay,
                (int)ErrorCodes.FileLengthOutOfBoundsError);
            
            responseModelDto.Message = $"File {file.FileName}'s length must be ranging from {Constants.MinImageFileUploadSize} to {Constants.MaxImageFileUploadSize} (Err: {(int)ErrorCodes.FileLengthOutOfBoundsError}).";

            return UnprocessableEntity();
        }
        
        // Path construction
        var trustedFileNameForFileStorage = Path.GetRandomFileName();
        if (!file.File.ContentType.TryGetDefaultExtension(out var fileExtension))
        {
            _logger.Warning("File {FileName}'s MIMO type doesn't match any file extensions. (Err: {ErrorCode}).",
                trustedFileNameForDisplay,
                (int)ErrorCodes.FileLengthOutOfBoundsError);

            responseModelDto.Message = $"File {file.FileName}'s MIMO type doesn't match any file extensions. (Err: {(int)ErrorCodes.FileMimoTypeToExtensionError}).";

            return UnprocessableEntity();
        }

        try
        {
            var path = Path.Combine(_hostEnvironment.ContentRootPath,
                "Development", 
                "Uploads",
                $"{trustedFileNameForFileStorage}{fileExtension}");
        
            // Create file at path
            await using FileStream fs = new(path, FileMode.Create);
            await file.File.CopyToAsync(fs);

            _logger.Information("{FileName} saved at {Path}",
                trustedFileNameForDisplay, 
                path);
            
            // Update database
            var imageFileEntities = await _imageFileRepository.GetAllEntitiesAsync(true);
            var imageFileEntitiesAsArray = imageFileEntities as ImageFileEntity[] ?? imageFileEntities.ToArray();
            var match = imageFileEntitiesAsArray.SingleOrDefault(e => e.CarouselItemId == id);
            if (match is not null)
            {
                if (System.IO.File.Exists(match.Path)) System.IO.File.Delete(match.Path);
                
                match.Path = path;
                _imageFileRepository.UpdateEntity(match);
                _logger.Information("{EntityName} associated with carousel item with id: {ID} has been updated .",
                    nameof(ImageFileEntity),
                    id);
            }
            else
            {
                var imageFileEntity = new ImageFileEntity
                {
                    Path = path,
                    CarouselItemId = id
                };

                _imageFileRepository.CreateEntity(imageFileEntity);
                _logger.Information("{EntityName} associated with carousel item with id: {ID} has been created.",
                    nameof(ImageFileEntity),
                    id);
            }

            await _imageFileRepository.SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.Error("{FileName} error on upload (Err: {ErrorCode}): {Message}",
                trustedFileNameForDisplay, 
                (int)ErrorCodes.FileOnUploadError,
                ex.Message);

            responseModelDto.Message = $"File {file.FileName} error on upload (Err: {(int)ErrorCodes.FileOnUploadError}): {ex.Message}";

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        responseModelDto.IsSuccess = true;
        responseModelDto.Message = $"File {file.FileName} has been successfully uploaded.";

        return Created(resourcePath, new { responseModelDto.IsResponse, responseModelDto.Message, responseModelDto.IsSuccess });
    }
}