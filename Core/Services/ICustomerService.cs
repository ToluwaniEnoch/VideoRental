using Api.Models.Payloads;
using Api.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
    public interface ICustomerService
    {
        Task<ApiResponse<CustomerResponse>> RegisterAsync(CreateCustomerPayload payload);
        Task<StatusResponse> UpdateCustomerProfileAsync(CustomerUpdatePayload payload);
        Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(Guid customerId);
        Task<ApiResponse<ListResponse<CustomerResponse>>> GetAllCustomers(CustomerFilterPayload payload, int skip = 0, int limit = 20);

    }
}
