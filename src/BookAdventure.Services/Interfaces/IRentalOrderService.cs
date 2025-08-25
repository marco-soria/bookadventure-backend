using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Services.Interfaces.Base;

namespace BookAdventure.Services.Interfaces;

public interface IRentalOrderService : IBaseService<RentalOrder>
{
    Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetAsync(PaginationDto pagination);
    Task<BaseResponseGeneric<RentalOrderResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<int>> CreateRentalOrderAsync(RentalOrderRequestDto request);
    Task<BaseResponse> UpdateAsync(int id, RentalOrderRequestDto request);
    Task<BaseResponse> DeleteAsync(int id);
    Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetByCustomerAsync(int customerId, PaginationDto pagination);
    Task<BaseResponse> ReturnBooksAsync(int rentalOrderId, List<int> bookIds);
    Task<BaseResponseGeneric<ICollection<RentalOrderResponseDto>>> GetOverdueRentalsAsync(PaginationDto pagination);
}
