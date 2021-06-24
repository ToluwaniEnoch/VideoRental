using Api.Data;
using Api.Data.Entities.Account;
using Api.Data.Entities.Parties;
using Api.Models.Constants;
using Api.Models.Payloads;
using Api.Models.Responses;
using Api.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Api.Services
{
    internal class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly ISessionService sessionService;

        public CustomerService(ILogger<CustomerService> logger, IUnitOfWork unitOfWork, UserManager<Persona> userManager, ISessionService sessionService, AppDbContext dbContext)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.sessionService = sessionService;
        }

        public async Task<ApiResponse<CustomerResponse>> RegisterAsync(CreateCustomerPayload payload)
        {
            logger.LogInformation($"Registration initiated for customer with {payload.CustomerEmail}");                       

            if (string.IsNullOrEmpty(payload.CustomerEmail) || string.IsNullOrEmpty(payload.CustomerPhoneNumber))
            {
                return new ApiResponse<CustomerResponse>("Email and Phone number must be provided") { Code = ResponseCodes.NoData};
            }

            if (string.IsNullOrEmpty(payload.TIN) || string.IsNullOrEmpty(payload.RcNumber))
            {
                return new ApiResponse<CustomerResponse>( "TIN and RcNumber must be provided") { Code = ResponseCodes.NoData};
            }

            var customerExists = await unitOfWork.CustomerRepository.CustomerExists(payload.TIN, payload.RcNumber);
            if (customerExists)
                return new ApiResponse<CustomerResponse>("Customer already exist")
                {
                    Code = ResponseCodes.ResourceAlreadyExist,
                };

            var customer = new Customer(payload.TIN, payload.TIN, payload.AccountNumber, payload.CustomerEmail, payload.CustomerPhoneNumber, payload.CompanyName, payload.Address);
            
            await unitOfWork.CustomerRepository.CreateAsync(customer);           

            return new ApiResponse<CustomerResponse>("") {
                Code = ResponseCodes.Success,
                Data = new CustomerResponse(customer.Id, customer.CompanyName, customer.TIN, customer.RcNumber, customer.PhoneNumber, customer.Email, customer.AccountNumber)

            };
        }
    
        public async Task<StatusResponse> UpdateCustomerProfileAsync(CustomerUpdatePayload payload)
        {
            // since a customer does not login, someone has to have permission to edit a customer's details
            var session = sessionService.GetSession();
            if (session is null) return new StatusResponse("User is not authenticated") { Code = ResponseCodes.Unauthenticated };

            logger.LogInformation($"Customer profile update initiated by {session.Email}");
            if (!sessionService.HasPermission(Permissions.CanEditCustomerDetails))
            {
                return new StatusResponse("User does not have permission to edit customer's details") { Code = ResponseCodes.NoPermission};
            }

            var user = await unitOfWork.CustomerRepository.GetByIdAsync(payload.CustomerId);

            if (user is null)
            {
                return new StatusResponse("Cannot find user") { Code = ResponseCodes.NoData };
            }

            await unitOfWork.CustomerRepository.UpdateAsync(user);            

            return new StatusResponse("Customer details updated") { Code = ResponseCodes.Success};

        }
        
        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(Guid customerId)
        {
            var session = sessionService.GetSession();
            if (session is null) return new ApiResponse<CustomerResponse>("User is not authenticated") { Code = ResponseCodes.Unauthenticated };

            if (!sessionService.HasPermission("CanViewCustomerDetails")) return new ApiResponse<CustomerResponse>("User does not have permission ") { Code = ResponseCodes.NoPermission };

            var customer = await unitOfWork.CustomerRepository.GetByIdAsync(customerId);

            if (customer is null)
                return new ApiResponse<CustomerResponse>("Customer does not exist")
                {
                    Code = ResponseCodes.NoData,
                };

            return new ApiResponse<CustomerResponse>( "Customer Retreived Successfully")
            {
                Code = ResponseCodes.Success,
                Data = new CustomerResponse(customer.Id, customer.CompanyName, customer.TIN, customer.RcNumber, customer.PhoneNumber, customer.Email, customer.AccountNumber)
            };
        }

        public async Task<ApiResponse<ListResponse<CustomerResponse>>> GetAllCustomers(CustomerFilterPayload payload, int skip = 0, int limit = 20)
        {
            var session = sessionService.GetSession();
            if (session is null) return new ApiResponse<ListResponse<CustomerResponse>>("User is not authenticated") { Code = ResponseCodes.Unauthenticated };

            if (!sessionService.HasPermission(Permissions.CanEditCustomerDetails)) return new ApiResponse<ListResponse<CustomerResponse>>("User does not have permission to view customers") { Code = ResponseCodes.NoPermission };

            var customersQuery = unitOfWork.CustomerRepository.GetQuery(payload);

            return new ApiResponse<ListResponse<CustomerResponse>>("customers retrieved")
            {
                Data = new ListResponse<CustomerResponse>(await customersQuery.LongCountAsync(), await customersQuery
                   .Skip(skip).Take(limit)
                   .Select(x => new CustomerResponse(x.Id, x.CompanyName, x.TIN, x.RcNumber, x.PhoneNumber, x.Email, x.AccountNumber)).ToListAsync())
            };
         }

        

    }
}
