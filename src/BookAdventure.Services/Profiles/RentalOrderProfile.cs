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
            .ForMember(dest => dest.CustomerDNI, opt => opt.MapFrom(src => src.Customer.DNI))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.RentalOrderDetails))
            .ForMember(dest => dest.TotalBooks, opt => opt.MapFrom(src => src.RentalOrderDetails.Sum(d => d.Quantity)))
            .ForMember(dest => dest.ActiveBooks, opt => opt.MapFrom(src => src.RentalOrderDetails.Where(d => !d.IsReturned).Sum(d => d.Quantity)))
            .ForMember(dest => dest.ReturnedBooks, opt => opt.MapFrom(src => src.RentalOrderDetails.Where(d => d.IsReturned).Sum(d => d.Quantity)))
            .ForMember(dest => dest.HasOverdueBooks, opt => opt.MapFrom(src => src.RentalOrderDetails.Any(d => !d.IsReturned && d.DueDate < DateTime.Now)));

        // RentalOrderDetailRequestDto to RentalOrderDetail
        CreateMap<RentalOrderDetailRequestDto, RentalOrderDetail>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DueDate, opt => opt.Ignore()) // Calculated from RentalDays
            .ForMember(dest => dest.ReturnDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsReturned, opt => opt.Ignore())
            .ForMember(dest => dest.RentalOrderId, opt => opt.Ignore())
            .ForMember(dest => dest.RentalOrder, opt => opt.Ignore())
            .ForMember(dest => dest.Book, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // RentalOrderDetail to RentalOrderDetailResponseDto
        CreateMap<RentalOrderDetail, RentalOrderDetailResponseDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.BookAuthor, opt => opt.MapFrom(src => src.Book.Author))
            .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => src.Book.ISBN))
            .ForMember(dest => dest.BookImageUrl, opt => opt.MapFrom(src => src.Book.ImageUrl))
            .ForMember(dest => dest.BookGenre, opt => opt.MapFrom(src => src.Book.Genre.Name))
            .ForMember(dest => dest.RentalOrderNumber, opt => opt.MapFrom(src => src.RentalOrder.OrderNumber))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => !src.IsReturned && src.DueDate < DateTime.Now));
    }
}
