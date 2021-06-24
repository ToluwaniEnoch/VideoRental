//using Api.Data;
//using Api.Data.Entities.Account;
//using Api.Models.Payloads;
//using Api.Models.Responses;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Api.Services
//{
//    internal class AccountService : IAccountService
//    {
//        private readonly UserManager<Persona> _userManager;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly ILogger _logger;

//        public AccountService(UserManager<Persona> userManager, IUnitOfWork unitOfWork, ILogger logger)
//        {
//            _userManager = userManager;
//            _unitOfWork = unitOfWork;
//            _logger = logger;
//        }
//        public async Task<ApiResponse<AccountResponse>> RegisterAsync(CreateAccountPayload payload)
//        {
//            //logger.LogInformation("Persona Registration initiated for {0}", payload.Email);
//            Persona account = null;
//            if (!string.IsNullOrEmpty(payload.Email) )
//            {
//                account = await _userManager.Users.FirstOrDefaultAsync(c => c.NormalizedEmail == payload.Email.ToUpper());
//            }
            
           

//            foreach (var passwordValidator in _userManager.PasswordValidators)
//            {
//                var result = await passwordValidator.ValidateAsync(_userManager, null, payload.Password);
//                if (result.Succeeded) continue;
//                _logger.LogError("User Registration Password Validation Failed");
//                return new ApiResponse<AccountResponse>("") { Code = 40,  };
//            }

//            var passwordHasher = _userManager.PasswordHasher;

//            var person = new Persona(payload.Email, payload.FirstName, payload.LastName)
            

//            var hashPassword = passwordHasher.HashPassword(person, payload.Password);

//            person.PasswordHash = hashPassword;            

//            await _unitOfWork.AccountRepository.AddAsync(person);

//            if (!await dbContext.TrySaveChangesAsync())
//            {
//                logger.LogError("User Registration Failed");
//                return new ObjectResource<PersonaRegisterResource>
//                {
//                    Code = ResourceCodes.ServiceError,
//                    Message = "Error Occurred Registering User"
//                };
//            }

//            logger.LogInformation("User Registered Successfully");

//            if (!sendNotification && isSuperAdmin)
//            {
//                await distributedCache.SetStringAsync($"{person.Id}-password", payload.Password,
//                    new DistributedCacheEntryOptions
//                    {
//                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
//                    });
//            }
//            else
//            {
//                _ = mediator.Publish(new PersonRegisteredNotification { Person = person, WasCreatedBySuperAdmin = isSuperAdmin, DefaultPassword = isSuperAdmin ? payload.Password : null });
//            }

//            return new ObjectResource<PersonaRegisterResource> { Data = new PersonaRegisterResource { PersonaId = person.Id } };
//        }
//    }
//    }
//}
