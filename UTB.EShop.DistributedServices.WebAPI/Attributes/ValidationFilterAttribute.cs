using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ILogger = Serilog.ILogger;

namespace UTB.EShop.DistributedServices.WebAPI.Attributes;

public class ValidationFilterAttribute : IActionFilter
{
    private readonly ILogger _logger;
    
    public ValidationFilterAttribute(ILogger logger)
    {
        _logger = logger;
    }
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];
        var param = context.ActionArguments
            .SingleOrDefault(x => x.Value!.ToString()!.Contains("Dto")).Value;
        
        if (param == null)
        {
            _logger.Warning($"Object sent from client is null. Controller: {controller}, action: {action}");
            context.Result = new BadRequestObjectResult($"Object is null. Controller: {controller}, action: {action}");
            return;
        }

        if (context.ModelState.IsValid) return;
        _logger.Warning($"Invalid model state for the object. Controller: {controller}, action: {action}");
        context.Result = new UnprocessableEntityObjectResult(context.ModelState);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}