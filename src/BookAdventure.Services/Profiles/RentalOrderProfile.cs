using AutoMapper;
using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;

namespace BookAdventure.Services.Profiles;

public class RentalOrderProfile : Profile
{
    public RentalOrderProfile()
    {
        // Request to Entity
        CreateMap<RentalOrderRequestDto, RentalOrder>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderNumber, opt => opt.Ignore()) // Generated automatically
            .ForMember(dest => dest.OrderDate, opt => opt.Ignore()) // Set automatically
            .ForMember(dest => dest.OrderStatus, opt => opt.Ignore()) // Set to default
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())            
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.RentalOrderDetails, opt => opt.MapFrom(src => src.Details));
            
        // Entity to Response
        CreateMap<RentalOrder, RentalOrderResponseDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.Email))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.RentalOrderDetails));
    }
}
