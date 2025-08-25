using AutoMapper;
using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;

namespace BookAdventure.Services.Profiles;

public class RentalOrderDetailProfile : Profile
{
    public RentalOrderDetailProfile()
    {
        // Request to Entity
        CreateMap<RentalOrderDetailRequestDto, RentalOrderDetail>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RentalOrderId, opt => opt.Ignore()) // Set by parent
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.RentalOrder, opt => opt.Ignore())
            .ForMember(dest => dest.Book, opt => opt.Ignore());
            
        // Entity to Response
        CreateMap<RentalOrderDetail, RentalOrderDetailResponseDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.BookAuthor, opt => opt.MapFrom(src => src.Book.Author))
            .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => src.Book.ISBN));
    }
}
