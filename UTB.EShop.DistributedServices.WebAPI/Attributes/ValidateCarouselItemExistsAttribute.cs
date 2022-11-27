using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UTB.EShop.Application.Interfaces.Repositories;
using UTB.EShop.Infrastructure.Entities;
using ILogger = Serilog.ILogger;

namespace UTB.EShop.DistributedServices.WebAPI.Attributes;

public class ValidateCarouselItemExistsAttribute : IAsyncActionFilter
{
    private readonly IRepository<CarouselItemEntity> _repository;
    private readonly ILogger _logger;

    public ValidateCarouselItemExistsAttribute(IRepository<CarouselItemEntity> repository, ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        var trackChanges = method.Equals("PUT") || method.Equals("PATCH");
        var id = (int)context.ActionArguments["id"]!;
        
        var carouselItemEntity = await _repository.GetEntityAsync(id, trackChanges);
        if (carouselItemEntity == null)
        {
            _logger.Warning($"Carousel item with id: {id} doesn't exist in the database.");
            context.Result = new NotFoundResult();
        }
        else
        {
            context.HttpContext.Items.Add("carouselItem", carouselItemEntity);
            await next();
        }
    }
}