using AutoMapper;
using UTB.EShop.Application.DataTransferObjects.Identity;
using UTB.EShop.Infrastructure.Identity;

namespace UTB.EShop.Infrastructure.Mappings;

public sealed class IdentityProfile : Profile
{
    public IdentityProfile()
    {
        CreateMap<UserForRegistrationDto, User>();
    }
}