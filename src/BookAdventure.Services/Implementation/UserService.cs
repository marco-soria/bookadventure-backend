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
using Microsoft.EntityFrameworkCore;
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
    private readonly ICustomerService _customerService;

    public UserService(
        UserManager<BookAdventureUserIdentity> userManager,
        SignInManager<BookAdventureUserIdentity> signInManager,
        IConfiguration configuration,
        ILogger<UserService> logger,
        IMapper mapper,
        ICustomerService customerService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
        _customerService = customerService;
    }

    public async Task<BaseResponseGeneric<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        var response = new BaseResponseGeneric<RegisterResponseDto>();

        try
        {
            // First check if customer with same DNI or email already exists
            var existingCustomerByDni = await _customerService.GetByDniAsync(request.DocumentNumber);
            if (existingCustomerByDni.Success)
            {
                response.Success = false;
                response.ErrorMessage = "A customer with this DNI already exists";
                return response;
            }

            var existingCustomers = await _customerService.FindAsync(c => c.Email == request.Email);
            if (existingCustomers.Any())
            {
                response.Success = false;
                response.ErrorMessage = "A customer with this email already exists";
                return response;
            }

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

                // Create corresponding Customer entity
                var customerRequest = new CustomerRequestDto
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    DNI = request.DocumentNumber,
                    Age = request.Age,
                    PhoneNumber = null // Can be updated later
                };

                // Map to Customer entity and set UserId
                var customer = _mapper.Map<Customer>(customerRequest);
                customer.UserId = user.Id;
                
                try
                {
                    var createdCustomer = await _customerService.CreateAsync(customer);
                    
                    response.Data = new RegisterResponseDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email!
                    };
                    response.Success = true;
                    
                    _logger.LogInformation("User registered successfully with Id: {UserId} and Customer Id: {CustomerId}", 
                        user.Id, createdCustomer.Id);
                }
                catch (Exception customerEx)
                {
                    // If customer creation fails, rollback user creation
                    await _userManager.DeleteAsync(user);
                    response.Success = false;
                    response.ErrorMessage = $"Error creating customer profile: {customerEx.Message}";
                    _logger.LogError(customerEx, "Error creating customer during user registration");
                    return response;
                }
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
                    var refreshToken = GenerateRefreshToken();
                    var lifetimeSeconds = Convert.ToInt32(_configuration["JWT:LifetimeInSeconds"] ?? "86400");
                    var refreshTokenLifetimeDays = Convert.ToInt32(_configuration["JWT:RefreshTokenLifetimeInDays"] ?? "30");
                    var expirationDate = DateTime.UtcNow.AddSeconds(lifetimeSeconds);
                    var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays);

                    // Update user with refresh token
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpirationDate = refreshTokenExpirationDate;
                    await _userManager.UpdateAsync(user);

                    response.Data = new LoginResponseDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email!,
                        Token = token,
                        ExpirationDate = expirationDate,
                        RefreshToken = refreshToken,
                        RefreshTokenExpirationDate = refreshTokenExpirationDate,
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
        var lifetimeSeconds = Convert.ToInt32(_configuration["JWT:LifetimeInSeconds"] ?? "86400");

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(lifetimeSeconds),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<BaseResponseGeneric<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var response = new BaseResponseGeneric<LoginResponseDto>();

        try
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

            if (user == null || user.RefreshTokenExpirationDate <= DateTime.UtcNow)
            {
                response.Success = false;
                response.ErrorMessage = "Invalid or expired refresh token";
                return response;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var newToken = GenerateJwtToken(user, roles);
            var newRefreshToken = GenerateRefreshToken();
            var lifetimeSeconds = Convert.ToInt32(_configuration["JWT:LifetimeInSeconds"] ?? "86400");
            var refreshTokenLifetimeDays = Convert.ToInt32(_configuration["JWT:RefreshTokenLifetimeInDays"] ?? "30");
            var expirationDate = DateTime.UtcNow.AddSeconds(lifetimeSeconds);
            var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays);

            // Update user with new refresh token
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpirationDate = refreshTokenExpirationDate;
            await _userManager.UpdateAsync(user);

            response.Data = new LoginResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Token = newToken,
                ExpirationDate = expirationDate,
                RefreshToken = newRefreshToken,
                RefreshTokenExpirationDate = refreshTokenExpirationDate,
                Roles = roles.ToList()
            };
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "Error occurred during token refresh";
            _logger.LogError(ex, "Error in RefreshTokenAsync");
        }

        return response;
    }
}
