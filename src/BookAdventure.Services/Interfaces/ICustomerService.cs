using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Services.Interfaces.Base;

namespace BookAdventure.Services.Interfaces;

public interface ICustomerService : IBaseService<Customer>
{
    Task<BaseResponseGeneric<ICollection<CustomerResponseDto>>> GetAsync(PaginationDto pagination);
    Task<BaseResponseGeneric<CustomerResponseDto>> GetAsync(int id);
    Task<BaseResponse> UpdateAsync(int id, CustomerUpdateRequestDto request);
    Task<BaseResponse> DeleteAsync(int id);
    Task<BaseResponseGeneric<CustomerResponseDto>> GetByDniAsync(string dni);
    Task<BaseResponseGeneric<CustomerResponseDto>> GetByUserIdAsync(string userId);
    Task<BaseResponseGeneric<ICollection<CustomerResponseDto>>> SearchByNameAsync(string namePattern);
    Task<BaseResponseGeneric<ICollection<RentedBookResponseDto>>> GetRentedBooksByDniAsync(string dni);
    
    // Métodos para manejo de eliminación lógica
    Task<BaseResponseGeneric<ICollection<CustomerResponseDto>>> GetDeletedCustomersAsync(PaginationDto pagination);
    Task<BaseResponse> RestoreCustomerAsync(int id);
    
    // Método para admin panel
    Task<BaseResponseGeneric<ICollection<CustomerResponseDto>>> GetAllCustomersForAdminAsync(PaginationDto pagination);
}
