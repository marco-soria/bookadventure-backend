using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BookAdventure.Services.Implementation;

public class UserService : IUserService
{
    private readonly UserManager<BookAdventureUserIdentity> _userManager;
    private readonly SignInManager<BookAdventureUserIdentity> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;

    public UserService(
        UserManager<BookAdventureUserIdentity> userManager,
        SignInManager<BookAdventureUserIdentity> signInManager,
        IConfiguration configuration,
        ILogger<UserService> logger,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BaseResponseGeneric<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        var response = new BaseResponseGeneric<RegisterResponseDto>();

        try
        {
            var user = new BookAdventureUserIdentity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                DNI = request.DocumentNumber,
                Age = request.Age,
                EmailConfirmed = true // Auto-confirm for simplicity
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                // Assign default role
                await _userManager.AddToRoleAsync(user, Constants.RoleCustomer);

                response.Data = new RegisterResponseDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!
                };
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "Error occurred during registration";
            _logger.LogError(ex, "Error in RegisterAsync");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var response = new BaseResponseGeneric<LoginResponseDto>();

        try
        {
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var token = GenerateJwtToken(user, roles);
                    var expirationDate = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JWT:DurationInDays"] ?? "7"));

                    response.Data = new LoginResponseDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email!,
                        Token = token,
                        ExpirationDate = expirationDate,
                        Roles = roles.ToList()
                    };
                    response.Success = true;
                }
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = "Invalid email or password";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "Error occurred during login";
            _logger.LogError(ex, "Error in LoginAsync");
        }

        return response;
    }

    private string GenerateJwtToken(BookAdventureUserIdentity user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("FirstName", user.FirstName),
            new("LastName", user.LastName)
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:JWTKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JWT:DurationInDays"] ?? "7")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
