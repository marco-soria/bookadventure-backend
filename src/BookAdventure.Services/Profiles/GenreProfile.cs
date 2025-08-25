using AutoMapper;
using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;

namespace BookAdventure.Services.Profiles;

public class GenreProfile : Profile
{
    public GenreProfile()
    {
        // Request to Entity
        CreateMap<GenreRequestDto, Genre>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Books, opt => opt.Ignore());
            
        // Entity to Response
        CreateMap<Genre, GenreResponseDto>()
            .ForMember(dest => dest.TotalBooks, opt => opt.MapFrom(src => src.Books.Count));
    }
}
