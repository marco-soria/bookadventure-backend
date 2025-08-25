using System;

namespace BookAdventure.Dto.Response;

public class RegisterResponseDto : LoginResponseDto
{
    public string UserId { get; set; }
}
