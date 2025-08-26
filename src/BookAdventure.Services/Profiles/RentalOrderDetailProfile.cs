using AutoMapper;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;

namespace BookAdventure.Services.Profiles;

public class RentalOrderDetailProfile : Profile
{
    public RentalOrderDetailProfile()
    {
        // Entity to Response
        CreateMap<RentalOrderDetail, RentalOrderDetailResponseDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.BookAuthor, opt => opt.MapFrom(src => src.Book.Author))
            .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => src.Book.ISBN));
    }
}
