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
            logger.LogInformation($"Registration initiated for customer with {payload.Email}");

            if (string.IsNullOrEmpty(payload.Email))
            {
                return new ApiResponse<CustomerResponse>("Email must be provided") { Code = ResponseCodes.NoData };
            }



            var customer = new Customer { FirstName = payload.FirstName, LastName = payload.LastName, Email =payload.Email };

            await unitOfWork.CustomerRepository.CreateAsync(customer);

            return new ApiResponse<CustomerResponse>("")
            {
                Code = ResponseCodes.Success,
                Data = new CustomerResponse(customer.FirstName, customer.LastName, customer.Email, customer.Id)

            };
        }



    }
}
