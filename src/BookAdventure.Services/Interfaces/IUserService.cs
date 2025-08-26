using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;

namespace BookAdventure.Services.Interfaces;

public interface IUserService
{
    Task<BaseResponseGeneric<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<BaseResponseGeneric<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<BaseResponseGeneric<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
}
