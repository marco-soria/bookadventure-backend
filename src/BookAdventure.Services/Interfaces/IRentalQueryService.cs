using BookAdventure.Services.Implementation;

namespace BookAdventure.Services.Interfaces;

public interface IRentalQueryService
{
    Task<List<RentalBookInfo>> GetRentedBooksByDniAsync(string dni, bool includeReturned = true, bool includeOverdue = true);
    Task<RentalSummary> GetRentalSummaryByDniAsync(string dni);
    Task<List<CustomerBasicInfo>> SearchCustomersByDniAsync(string dniPrefix);
}
