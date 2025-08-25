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

    public async Task<BaseResponseGeneric<int>> AddAsync(CustomerRequestDto request)
    {
        try
        {
            // Check if customer with same DNI or email already exists
            var existingByDni = await _customerRepository.GetByDniAsync(request.DNI);
            if (existingByDni != null)
            {
                return new BaseResponseGeneric<int>
                {
                    Success = false,
                    ErrorMessage = "Customer with this DNI already exists"
                };
            }

            var existingByEmail = await _customerRepository.FindAsync(c => c.Email == request.Email);
            if (existingByEmail.Any())
            {
                return new BaseResponseGeneric<int>
                {
                    Success = false,
                    ErrorMessage = "Customer with this email already exists"
                };
            }

            var customer = _mapper.Map<Customer>(request);
            var createdCustomer = await _customerRepository.CreateAsync(customer);

            return new BaseResponseGeneric<int>
            {
                Success = true,
                Data = createdCustomer.Id
            };
        }
        catch (Exception ex)
        {
            return new BaseResponseGeneric<int>
            {
                Success = false,
                ErrorMessage = $"Error creating customer: {ex.Message}"
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
}
