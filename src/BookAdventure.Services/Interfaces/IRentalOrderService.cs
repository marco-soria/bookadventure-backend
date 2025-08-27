using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Services.Interfaces.Base;

namespace BookAdventure.Services.Interfaces;

public interface IRentalOrderService : IBaseService<RentalOrder>
{
    Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetAsync(PaginationDto pagination);
    Task<BaseResponseGeneric<RentalOrderResponseDto>> GetAsync(int id);
    Task<RentalOrderCreationResponseDto> CreateRentalOrderAsync(RentalOrderRequestDto request);
    Task<BaseResponse> UpdateAsync(int id, RentalOrderUpdateRequestDto request);
    Task<BaseResponse> DeleteAsync(int id);
    Task<BaseResponse> CancelRentalOrderAsync(int id);
    Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetByCustomerAsync(int customerId, PaginationDto pagination);
    Task<BaseResponse> ReturnBooksAsync(int rentalOrderId, List<int> bookIds);
    Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetOverdueRentalsAsync(PaginationDto pagination);
    
    // Métodos para manejo de eliminación lógica
    Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetDeletedRentalOrdersAsync(PaginationDto pagination);
    Task<BaseResponse> RestoreRentalOrderAsync(int id);
    
    // Método para admin panel
    Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetAllRentalOrdersForAdminAsync(PaginationDto pagination);
}
