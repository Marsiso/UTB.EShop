using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UTB.EShop.Application.DataTransferObjects.Carousel;
using UTB.EShop.Application.Interfaces.Models;
using UTB.EShop.Application.Interfaces.Repositories;
using UTB.EShop.Application.Paging;
using UTB.EShop.DistributedServices.WebAPI.Attributes;
using UTB.EShop.Infrastructure.Entities;
using ILogger = Serilog.ILogger;

namespace UTB.EShop.DistributedServices.WebAPI.Controllers;

[ApiController]
[Route("[Controller]")]
public class CarouselController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IRepository<CarouselItemEntity> _repository;
    private readonly IDataShaper<CarouselItemDto> _dataShaper;

    public CarouselController(IMapper mapper, ILogger logger, IRepository<CarouselItemEntity> repository, IDataShaper<CarouselItemDto> dataShaper)
    {
        _mapper = mapper;
        _logger = logger;
        _repository = repository;
        _dataShaper = dataShaper;
    }

    [HttpGet(Name = "GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] CarouselItemParameters carouselItemParameters)
    {
        var carouselItems = await _repository.GetAllEntitiesAsync<CarouselItemParameters>(carouselItemParameters);
        if (carouselItems is null)
        {
            _logger.Warning("Carousel item objects haven't been found.");
            return NotFound();
        }
        
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(carouselItems.MetaData));
        
        var carouselItemsDto = _mapper.Map<IEnumerable<CarouselItemDto>>(carouselItems);
        
        return Ok(_dataShaper.ShapeData(carouselItemsDto, carouselItemParameters.Fields));
    }
    
    [HttpGet("{id:int}", Name = "GetById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        var carouselItem = await _repository.GetEntityAsync(id);
        if (carouselItem is null)
        {
            _logger.Warning("Carousel item object with id: {0} hasn't been found.", id.ToString());
            return NotFound();
        }

        var carouselItemDto = _mapper.Map<CarouselItemDto>(carouselItem);
        
        return Ok(carouselItemDto);
    }
    
    [HttpGet("Collection/({ids})", Name = "GetByIds")]
    public async Task<IActionResult> GetByIds(IEnumerable<int>? ids)
    {
        if(ids == null)
        {
            _logger.Warning("Collection of ids cannot be an null object.");
            return BadRequest("Collection of ids cannot be an null object.");
        }

        var enumerable = ids as int[] ?? ids.ToArray();
        var carouselItemEntities = await _repository.GetByIdsAsync(enumerable, trackChanges: 
            false);
        
        if(enumerable.Length != carouselItemEntities.Count())
        {
            _logger.Warning("Number of matches doesn't add up to the number of the given ids.");
            return NotFound();
        }
        var companiesToReturn = _mapper.Map<IEnumerable<CarouselItemDto>>(carouselItemEntities);
        return Ok(companiesToReturn);
    }

    [HttpDelete("{id:int}", Name = "Delete")]
    [ServiceFilter(typeof(ValidateCarouselItemExistsAttribute))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        var carouselItem = HttpContext.Items["carouselItem"] as CarouselItemEntity;
        
        _repository.DeleteEntity(carouselItem!);
        await _repository.SaveAsync();
        
        return NoContent();
    }
    
    [HttpPost(Name = "Post")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CarouselItemForCreationDto? carouselItem)
    {
        var carouselItemEntity = _mapper.Map<CarouselItemEntity>(carouselItem);
        
        _repository.CreateEntity(carouselItemEntity);
        await _repository.SaveAsync();

        var carouselItemToReturn = _mapper.Map<CarouselItemDto>(carouselItemEntity);
        
        return CreatedAtRoute("GetById", 
            new { id = carouselItemToReturn.Id }, carouselItemToReturn);
    }
    
    [HttpPost("Collection", Name = "PostCollection")]
    public async Task<IActionResult> PostCollection([FromBody] 
        IEnumerable<CarouselItemForCreationDto>? itemCollection)
    {
        if(itemCollection is null)
        {
            _logger.Warning("Carousel items collection sent from client can't be an null object.");
            return BadRequest("Carousel items collection sent from client can't be an null object.");
        }
        
        var itemEntities = _mapper.Map<IEnumerable<CarouselItemEntity>>(itemCollection);
        foreach (var itemEntity in itemEntities) _repository.CreateEntity(itemEntity);

        await _repository.SaveAsync();
        
        var collectionToReturn = _mapper.Map<IEnumerable<CarouselItemDto>>(itemEntities);
        var ids = string.Join(",", collectionToReturn.Select(dto => dto.Id));
        
        return CreatedAtRoute("GetByIds", new { ids }, 
            collectionToReturn);
    }

    [HttpPatch("{id:int}")]
    [ServiceFilter(typeof(ValidateCarouselItemExistsAttribute))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<CarouselItemForUpdateDto>? patchDocument)
    {
        if (patchDocument is null)
        {
            _logger.Warning("{0} cannot be an null object.", nameof(JsonPatchDocument<CarouselItemForUpdateDto>));
            return BadRequest("JsonPatchDocument<CarouselItemForUpdateDto> object sent from client is null.");
        }

        var carouselItemEntity = HttpContext.Items["carouselItem"] as CarouselItemEntity;
        
        var carouselItemToPatch = _mapper.Map<CarouselItemForUpdateDto>(carouselItemEntity);
        patchDocument.ApplyTo(carouselItemToPatch);

        _mapper.Map(carouselItemToPatch, carouselItemEntity);
        
        TryValidateModel(carouselItemToPatch);
        if (!ModelState.IsValid)
        {
            _logger.Warning($"Invalid model state for the {nameof(CarouselItemEntity)} object.");
            return UnprocessableEntity($"Invalid model state for the {nameof(CarouselItemEntity)} object.");   
        }

        await _repository.SaveAsync();

        return NoContent();
    }
    
    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ServiceFilter(typeof(ValidateCarouselItemExistsAttribute))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Put(int id, [FromBody] CarouselItemForUpdateDto? carouselItem)
    {
        var carouselItemEntity = HttpContext.Items["carouselItem"] as CarouselItemEntity;
        _mapper.Map(carouselItem, carouselItemEntity);
        await _repository.SaveAsync();

        return NoContent();
    }
}