using AutoMapper;
using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;

namespace BookAdventure.Services.Profiles;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        // Request to Entity
        CreateMap<CustomerRequestDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.FullName, opt => opt.Ignore()) // Computed property
            .ForMember(dest => dest.RentalOrders, opt => opt.Ignore());
            
        // Entity to Response
        CreateMap<Customer, CustomerResponseDto>()
            .ForMember(dest => dest.TotalRentalOrders, opt => opt.MapFrom(src => src.RentalOrders.Count))
            .ForMember(dest => dest.ActiveRentals, opt => opt.MapFrom(src => src.RentalOrders.Count(ro => ro.OrderStatus == OrderStatus.Active)))
            .ForMember(dest => dest.OverdueRentals, opt => opt.MapFrom(src => src.RentalOrders
                .Where(ro => ro.OrderStatus == OrderStatus.Active)
                .SelectMany(ro => ro.RentalOrderDetails)
                .Count(rod => rod.DueDate < DateTime.UtcNow && !rod.IsReturned)));
    }
}
