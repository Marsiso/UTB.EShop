using System;
using AutoMapper;
using UTB.EShop.Application.DataTransferObjects.Carousel;
using UTB.EShop.Infrastructure.Entities;

namespace UTB.EShop.Infrastructure.Mappings;

public sealed class CarouselItemProfile : Profile
{
    public CarouselItemProfile()
    {
        // Get a resource
        CreateMap<CarouselItemEntity, CarouselItemDto>().ReverseMap();
        
        // Create a resource
        CreateMap<CarouselItemForCreationDto, CarouselItemEntity>()
            .ForMember(item => item.CreatedBy, 
                opt => opt.MapFrom(_ => "Marek Olsak"))
            .ForMember(item => item.DateCreated, 
                opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(item => item.ModifiedBy, 
                opt => opt.MapFrom(_ => "Marek Olsak"))
            .ForMember(item => item.DateModified, 
                opt => opt.MapFrom(_ => DateTime.Now))
            .ReverseMap();
            
        // Update resource
        CreateMap<CarouselItemForUpdateDto, CarouselItemEntity>()
            .ForMember(item => item.CreatedBy, 
                opt => opt.MapFrom(_ => "Marek Olsak"))
            .ForMember(item => item.DateCreated, 
                opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(item => item.ModifiedBy, 
                opt => opt.MapFrom(_ => "Marek Olsak"))
            .ForMember(item => item.DateModified, 
                opt => opt.MapFrom(_ => DateTime.Now))
            .ReverseMap();
    }
}