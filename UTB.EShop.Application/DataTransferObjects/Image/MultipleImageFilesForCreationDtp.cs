using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UTB.EShop.Application.DataTransferObjects.Image;

public sealed class MultipleImageFilesForCreationDtp
{
    [Required(ErrorMessage = "Please select files")]
    public List<IFormFile> ImageFiles { get; set; }
}