using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UTB.EShop.Application.Constants;
using UTB.EShop.DistributedServices.WebAPI.Utility;
using ILogger = Serilog.ILogger;

namespace UTB.EShop.DistributedServices.WebAPI.Attributes;

/// <summary>
/// Specifies that the action parameters are required, each sent file is an image file which complies with file length requirements
/// and counts sent files. 
/// </summary>
public sealed class ValidateImageFilesAttribute : IActionFilter
{
    private readonly ILogger _logger;

    public ValidateImageFilesAttribute(ILogger logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.RouteData.Values["controller"];
        var action = context.RouteData.Values["action"];
        
        // Get files from action context
        var param = context.ActionArguments.SingleOrDefault(pair =>
        {
            var valueString = pair.Value?.ToString()?.ToLower();
            return valueString is not null && valueString.Contains("FormFile");
        }).Value;

        // Params null pointer check
        if (param is null)
        {
            _logger.Warning($"Object sent from client is null. Controller: {controller}, action: {action}");
            context.Result = new BadRequestResult();
            return;
        }

        // Params object cast
        if (param is not IEnumerable<IFormFile> formFiles)
        {
            _logger.Warning($"Object cast failure. Controller: {controller}, action: {action}");
            context.Result = new UnprocessableEntityResult();
            return;
        }

        IList<string> extensions = new List<string>();
        var len = 0;
        foreach (var formFile in formFiles)
        {
            // Content type validation
            if (formFile.ContentType.TryGetDefaultExtension(out var extension))
            {
                extensions.Add(extension!);
                
                // Counter
                len++;

                // File length validation
                if (formFile.Length is >= Constants.MaxImageFileUploadSize or <= Constants.MinImageFileUploadSize)
                {
                    _logger.Warning("Invalid file length. FileName: {FileName} FileLength: {FileLength} B must be ranging from {Min} B to {Man} B.",
                        formFile.Name, 
                        formFile.Length.ToString(),
                        Constants.MinImageFileUploadSize.ToString(), 
                        Constants.MaxImageFileUploadSize.ToString());
                    return;
                }

                continue;
            }
            
            _logger.Warning($"Media type not supported. FileName: {formFile.Name} MimoType: {formFile.ContentType}. Controller: {controller}, action: {action}");
            context.Result = new UnsupportedMediaTypeResult();
            return;
        }
        
        context.ActionArguments.Add("extensions", extensions);
        context.ActionArguments.Add("len", len);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}