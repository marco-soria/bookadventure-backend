using AutoMapper;
using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Repositories.Interfaces;
using BookAdventure.Repositories.Utils;
using BookAdventure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Services.Implementation;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponseGeneric<ICollection<CustomerResponseDto>>> GetAsync(PaginationDto pagination)
    {
        try
        {
            var customers = await _customerRepository.Query()
                .Paginate(pagination)
                .ToListAsync();

            var totalRecords = await _customerRepository.CountAsync();
            var response = _mapper.Map<ICollection<CustomerResponseDto>>(customers);

            return new BaseResponseGeneric<ICollection<CustomerResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<ICollection<CustomerResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving customers: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<CustomerResponseDto>> GetAsync(int id)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                return new BaseResponseGeneric<CustomerResponseDto>
                {
                    Success = false,
                    ErrorMessage = "Customer not found"
                };
            }

            var response = _mapper.Map<CustomerResponseDto>(customer);

            return new BaseResponseGeneric<CustomerResponseDto>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<CustomerResponseDto>
            {
                Success = false,
                ErrorMessage = $"Error retrieving customer: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> UpdateAsync(int id, CustomerUpdateRequestDto request)
    {
        try
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(id);
            if (existingCustomer == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Customer not found"
                };
            }

            // Check if another customer with the same DNI exists (only if DNI is provided)
            if (!string.IsNullOrEmpty(request.DNI))
            {
                var customerWithSameDni = await _customerRepository.GetByDniAsync(request.DNI);
                if (customerWithSameDni != null && customerWithSameDni.Id != id)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        ErrorMessage = "Another customer with this DNI already exists"
                    };
                }
                existingCustomer.DNI = request.DNI;
            }

            // Check if another customer with the same email exists (only if email is provided)
            if (!string.IsNullOrEmpty(request.Email))
            {
                var customerWithSameEmail = await _customerRepository.FindAsync(c => c.Email == request.Email);
                if (customerWithSameEmail.Any(c => c.Id != id))
                {
                    return new BaseResponse
                    {
                        Success = false,
                        ErrorMessage = "Another customer with this email already exists"
                    };
                }
                existingCustomer.Email = request.Email;
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(request.FirstName))
                existingCustomer.FirstName = request.FirstName;
            
            if (!string.IsNullOrEmpty(request.LastName))
                existingCustomer.LastName = request.LastName;
            
            if (request.Age.HasValue)
                existingCustomer.Age = request.Age.Value;
            
            if (!string.IsNullOrEmpty(request.PhoneNumber))
                existingCustomer.PhoneNumber = request.PhoneNumber;

            await _customerRepository.UpdateAsync(existingCustomer);

            return new BaseResponse { Success = true };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error updating customer: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        try
        {
            var success = await _customerRepository.SoftDeleteAsync(id);

            if (!success)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Customer not found"
                };
            }

            return new BaseResponse { Success = true };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error deleting customer: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<CustomerResponseDto>> GetByDniAsync(string dni)
    {
        try
        {
            var customer = await _customerRepository.GetByDniAsync(dni);

            if (customer == null)
            {
                return new BaseResponseGeneric<CustomerResponseDto>
                {
                    Success = false,
                    ErrorMessage = "Customer not found"
                };
            }

            var response = _mapper.Map<CustomerResponseDto>(customer);

            return new BaseResponseGeneric<CustomerResponseDto>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<CustomerResponseDto>
            {
                Success = false,
                ErrorMessage = $"Error retrieving customer by DNI: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<CustomerResponseDto>> GetByUserIdAsync(string userId)
    {
        try
        {
            // Use FindAsync to get customer by UserId
            var customers = await _customerRepository.FindAsync(c => c.UserId == userId);
            var customer = customers.FirstOrDefault();

            if (customer == null)
            {
                return new BaseResponseGeneric<CustomerResponseDto>
                {
                    Success = false,
                    ErrorMessage = "Customer not found for this user"
                };
            }

            // Get customer with RentalOrders using Query method
            var customerWithRentals = await _customerRepository.Query()
                .Where(c => c.Id == customer.Id)
                .Include(c => c.RentalOrders)
                .ThenInclude(ro => ro.RentalOrderDetails)
                .FirstOrDefaultAsync();

            if (customerWithRentals == null)
            {
                return new BaseResponseGeneric<CustomerResponseDto>
                {
                    Success = false,
                    ErrorMessage = "Customer not found"
                };
            }

            var response = _mapper.Map<CustomerResponseDto>(customerWithRentals);

            return new BaseResponseGeneric<CustomerResponseDto>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<CustomerResponseDto>
            {
                Success = false,
                ErrorMessage = $"Error retrieving customer by User ID: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<ICollection<CustomerResponseDto>>> SearchByNameAsync(string namePattern)
    {
        try
        {
            var customers = await _customerRepository.SearchByNameAsync(namePattern);
            var response = _mapper.Map<ICollection<CustomerResponseDto>>(customers);

            return new BaseResponseGeneric<ICollection<CustomerResponseDto>>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<ICollection<CustomerResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error searching customers: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponseGeneric<ICollection<RentedBookResponseDto>>> GetRentedBooksByDniAsync(string dni)
    {
        try
        {
            // First find the customer by DNI
            var customer = await _customerRepository.GetByDniAsync(dni);
            if (customer == null)
            {
                return new BaseResponseGeneric<ICollection<RentedBookResponseDto>>
                {
                    Success = false,
                    ErrorMessage = "Customer not found with the provided DNI"
                };
            }

            // Get all rental order details for this customer with book information
            var rentalDetails = await _customerRepository.Query()
                .Where(c => c.DNI == dni)
                .SelectMany(c => c.RentalOrders)
                .SelectMany(ro => ro.RentalOrderDetails)
                .Include(rod => rod.Book)
                .ThenInclude(b => b.Genre)
                .Include(rod => rod.RentalOrder)
                .ToListAsync();

            var rentedBooks = rentalDetails.Select(rod => new RentedBookResponseDto
            {
                BookId = rod.BookId,
                Title = rod.Book.Title,
                Author = rod.Book.Author,
                ISBN = rod.Book.ISBN,
                Genre = rod.Book.Genre.Name,
                ImageUrl = rod.Book.ImageUrl,
                RentalOrderId = rod.RentalOrderId,
                OrderNumber = rod.RentalOrder.OrderNumber,
                OrderDate = rod.RentalOrder.OrderDate,
                DueDate = rod.DueDate,
                ReturnDate = rod.ReturnDate,
                IsReturned = rod.IsReturned,
                Quantity = rod.Quantity,
                RentalDays = rod.RentalDays,
                Notes = rod.Notes,
                OrderStatus = rod.RentalOrder.OrderStatus.ToString()
            }).ToList();

            return new BaseResponseGeneric<ICollection<RentedBookResponseDto>>
            {
                Success = true,
                Data = rentedBooks
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<ICollection<RentedBookResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving rented books: {ex.Message}"
            };
        }
    }

    // Base service implementations
    public async Task<IEnumerable<Customer>> GetAllAsync() => await _customerRepository.GetAllAsync();
    public async Task<IEnumerable<Customer>> GetAllIncludingDeletedAsync() => await _customerRepository.GetAllIncludingDeletedAsync();
    public async Task<Customer?> GetByIdAsync(int id) => await _customerRepository.GetByIdAsync(id);
    public async Task<Customer?> GetByIdIncludingDeletedAsync(int id) => await _customerRepository.GetByIdIncludingDeletedAsync(id);
    public async Task<IEnumerable<Customer>> FindAsync(System.Linq.Expressions.Expression<Func<Customer, bool>> predicate) => await _customerRepository.FindAsync(predicate);
    public async Task<Customer> CreateAsync(Customer entity) => await _customerRepository.CreateAsync(entity);
    public async Task<Customer?> UpdateAsync(Customer entity) => await _customerRepository.UpdateAsync(entity);
    public async Task<bool> SoftDeleteAsync(int id) => await _customerRepository.SoftDeleteAsync(id);
    public async Task<bool> RestoreAsync(int id) => await _customerRepository.RestoreAsync(id);
    public async Task<bool> HardDeleteAsync(int id) => await _customerRepository.HardDeleteAsync(id);
    public async Task<bool> ExistsAsync(int id) => await _customerRepository.ExistsAsync(id);
    public async Task<int> CountAsync() => await _customerRepository.CountAsync();
    public async Task<int> CountIncludingDeletedAsync() => await _customerRepository.CountIncludingDeletedAsync();

    /// <summary>
    /// Obtiene todos los clientes eliminados lógicamente con paginación
    /// </summary>
    public async Task<BaseResponseGeneric<ICollection<CustomerResponseDto>>> GetDeletedCustomersAsync(PaginationDto pagination)
    {
        try
        {
            var query = _customerRepository.QueryIncludingDeleted()
                .Where(c => c.Status == EntityStatus.Deleted);

            var totalRecords = await query.CountAsync();

            var customers = await query
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage)
                .ToListAsync();

            var response = _mapper.Map<ICollection<CustomerResponseDto>>(customers);

            return new BaseResponseGeneric<ICollection<CustomerResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<ICollection<CustomerResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving deleted customers: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Restaura un cliente eliminado lógicamente
    /// </summary>
    public async Task<BaseResponse> RestoreCustomerAsync(int id)
    {
        try
        {
            var customer = await _customerRepository.GetByIdIncludingDeletedAsync(id);
            if (customer == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Customer not found"
                };
            }

            if (customer.Status != EntityStatus.Deleted)
            {
                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "Customer is not deleted"
                };
            }

            // Verificar si ya existe otro cliente activo con el mismo DNI
            if (!string.IsNullOrEmpty(customer.DNI))
            {
                var existingCustomerWithSameDni = await _customerRepository.GetByDniAsync(customer.DNI);
                if (existingCustomerWithSameDni != null && existingCustomerWithSameDni.Id != id)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        ErrorMessage = "Another active customer with the same DNI already exists"
                    };
                }
            }

            var result = await _customerRepository.RestoreAsync(id);
            if (result)
            {
                return new BaseResponse
                {
                    Success = true
                };
            }

            return new BaseResponse
            {
                Success = false,
                ErrorMessage = "Failed to restore customer"
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error restoring customer: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtiene todos los clientes para el panel de admin (incluyendo eliminados) con paginación
    /// </summary>
    public async Task<BaseResponseGeneric<ICollection<CustomerResponseDto>>> GetAllCustomersForAdminAsync(PaginationDto pagination)
    {
        try
        {
            var query = _customerRepository.QueryIncludingDeleted()
                .Include(c => c.RentalOrders)
                .OrderBy(c => c.Id); // Orden consistente

            var totalRecords = await query.CountAsync();

            var customers = await query
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage)
                .ToListAsync();

            var response = customers.Select(customer => new CustomerResponseDto
            {
                Id = customer.Id,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                FullName = customer.FullName,
                DNI = customer.DNI,
                Age = customer.Age,
                PhoneNumber = customer.PhoneNumber,
                UserId = customer.UserId,
                Status = customer.Status == EntityStatus.Active ? "Active" : "Deleted",
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                TotalRentalOrders = customer.RentalOrders?.Count ?? 0,
                ActiveRentals = customer.RentalOrders?.Count(r => r.Status == EntityStatus.Active) ?? 0,
                OverdueRentals = 0 // Por ahora 0, se puede calcular después
            }).ToList();

            return new BaseResponseGeneric<ICollection<CustomerResponseDto>>
            {
                Success = true,
                Data = response,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<ICollection<CustomerResponseDto>>
            {
                Success = false,
                ErrorMessage = $"Error retrieving all customers for admin: {ex.Message}"
            };
        }
    }
}
