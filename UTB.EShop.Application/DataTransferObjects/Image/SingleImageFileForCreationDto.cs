using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using UTB.EShop.Infrastructure.Models;

namespace UTB.EShop.Application.DataTransferObjects.Image;

public sealed class SingleImageFileForCreationDto
{
    [Required(ErrorMessage = "Please enter file name.")]
    public string? FileName { get; set; }
    
    [Required(ErrorMessage = "Please select file.")]
    public IFormFile? File{ get; set; }
}