//using Api.Data;
//using Api.Data.Entities.Account;
//using Api.Data.Entities.Parties;
//using Api.Models.Constants;
//using Api.Models.Internals;
//using Api.Models.Payloads;
//using Api.Services;
//using Api.Services.Auth;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using Moq;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Test.Helpers;
//using Xunit;

//namespace Test.Services.Customers
//{
//    public class CustomerServiceTests : BaseTest
//    {
//        #region RegisterCustomer
//        [Fact]
//        public async void RegisterCustomer_PassValidPayload_ReturnSuccess()
//        {
//            var collection = GetCollection().BuildServiceProvider();
//            var service = collection.GetService<ICustomerService>();
//            var userManager = collection.GetService<UserManager<Persona>>();
//            var roleManager = collection.GetService<RoleManager<Role>>();

//            var result = service.RegisterAsync(new CreateCustomerPayload ("Innovantics", "1234565434", "TR51232", "+2349084453844", "jamesdoe@mailinator.com", "9898767623", "Lagos")).Result;
//            Assert.NotNull(result);
//            Assert.Equal(ResponseCodes.Success, result.Code);


//        }

//        [Fact]
//        public async void RegisterCustomer_NoTinInPayload_ReturnFailure()
//        {
//            var collection = GetCollection().BuildServiceProvider();
//            var service = collection.GetService<ICustomerService>();
//            var userManager = collection.GetService<UserManager<Persona>>();
//            var roleManager = collection.GetService<RoleManager<Role>>();


//            var result = service.RegisterAsync(new CreateCustomerPayload("Innovantics", "", "TR51232", "+2349084453844", "jamesdoe@mailinator.com", "9898767623", "Lagos")).Result;
//            Assert.NotNull(result);
//            Assert.Equal(ResponseCodes.NoData, result.Code);

//        }

//        [Fact]
//        public async void RegisterCustomer_NoRcNumberInPayload_ReturnFailure()
//        {
//            var collection = GetCollection().BuildServiceProvider();
//            var service = collection.GetService<ICustomerService>();
//            var userManager = collection.GetService<UserManager<Persona>>();
//            var roleManager = collection.GetService<RoleManager<Role>>();


//            var result = service.RegisterAsync(new CreateCustomerPayload("Innovantics", "1234565434", "", "+2349084453844", "jamesdoe@mailinator.com", "9898767623", "Lagos")).Result;
//            Assert.NotNull(result);
//            Assert.Equal(ResponseCodes.NoData, result.Code);

//        }
//        #endregion

//        #region UpdateCustomer

//        [Fact]
//        public async Task UpdateCustomer_ValidPayload_ReturnSuccess()
//        {
//            TestRef<Session> testRef = new TestRef<Session>();

//            var collection = GetCollection().AddTransient(xy =>
//            {
//                var moqObj = new Mock<ISessionService>();
//                moqObj.Setup(x => x.HasPermission(It.IsAny<string>())).Returns<string>(c => !string.IsNullOrEmpty(c));
//                moqObj.Setup(x => x.GetSession()).Returns(testRef.GetRef);
//                return moqObj.Object;
//            }).BuildServiceProvider();

//            var userManager = collection.GetService<UserManager<Persona>>()!;
//            var user = new Persona ( "jethro.daniel@innovantics.com", "Jethro", "Daniel" );
//            var xy = await userManager.CreateAsync(user, "Password123#");

//            var roleManager = collection.GetService<RoleManager<Role>>()!;
//            var appDbContext = collection.GetService<AppDbContext>()!;

//            var role = new Role { Name = "Admin", Permissions = new List<string> {  } };
//            await roleManager.CreateAsync(role);

//            await userManager.AddToRoleAsync(user, role.Name);
//            var customer = new Customer("1234565434", "TR51232", "9898767623", "jamesdoe@mailinator.com", "2349084453844", "Innovantics", "Lagos");
            
//            await appDbContext.Customers.AddAsync(customer);
//            await appDbContext.SaveChangesAsync();
//            appDbContext.Entry(customer).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            

//            testRef.GetRef = () => new Session(user.Id, user.FirstName, user.Email, role.Name, role.Id);
//            var service = collection.GetService<ICustomerService>();        

            
 
//            var result = await service.UpdateCustomerProfileAsync(new CustomerUpdatePayload("+2349084453844", "9898767623", "Oyo", customer.Id));
//            Assert.NotNull(result);
//            Assert.Equal(ResponseCodes.Success, result.Code);
//        }
//        #endregion

//        #region GetCustomerById
//        [Fact]

//        public async void GetCustomer_ValidPayload_ReturnSuccess()
//        {
//            TestRef<Session> testRef = new TestRef<Session>();

//            var collection = GetCollection().AddTransient(xy =>
//            {
//                var moqObj = new Mock<ISessionService>();
//                moqObj.Setup(x => x.HasPermission(It.IsAny<string>())).Returns<string>(c => !string.IsNullOrEmpty(c));
//                moqObj.Setup(x => x.GetSession()).Returns(testRef.GetRef);
//                return moqObj.Object;
//            }).BuildServiceProvider();

//            var userManager = collection.GetService<UserManager<Persona>>();
//            var user = new Persona("jethro.daniel@innovantics.com", "Jethro", "Daniel");
//            var xy = await userManager.CreateAsync(user, "Password123#");

//            var roleManager = collection.GetService<RoleManager<Role>>();
//            var appDbContext = collection.GetService<AppDbContext>();

//            var role = new Role { Name = "Admin", Permissions = new List<string> { } };
//            await roleManager.CreateAsync(role);

//            await userManager.AddToRoleAsync(user, role.Name);
//            await appDbContext.SaveChangesAsync();

//            testRef.GetRef = () => new Session(user.Id, user.FirstName, user.Email, role.Name, role.Id);
//            var service = collection.GetService<ICustomerService>();
//            var customer = new CreateCustomerPayload("Innovantics", "1234565434", "TR51232", "+2349084453844", "jamesdoe@mailinator.com", "9898767623", "Lagos");
//            var register = service.RegisterAsync(customer).Result;

//            var result = service.GetCustomerByIdAsync(register.Data.CustomerId).Result;
//            Assert.Equal(ResponseCodes.Success, result.Code);
//        }

//        #endregion

//        #region GetAllCustomers
//        [Fact]
//        public async void GetCustomers_PassValidPayload_ReturnSuccess()
//        {
//            TestRef<Session> testRef = new TestRef<Session>();

//            var collection = GetCollection().AddTransient(xy =>
//            {
//                var moqObj = new Mock<ISessionService>();
//                moqObj.Setup(x => x.HasPermission(It.IsAny<string>())).Returns<string>(c => !string.IsNullOrEmpty(c));
//                moqObj.Setup(x => x.GetSession()).Returns(testRef.GetRef);
//                return moqObj.Object;
//            }).BuildServiceProvider();

//            var userManager = collection.GetService<UserManager<Persona>>();
//            var user = new Persona("jethro.daniel@innovantics.com", "Jethro", "Daniel");
//            var xy = await userManager.CreateAsync(user, "Password123#");

//            var roleManager = collection.GetService<RoleManager<Role>>();
//            var appDbContext = collection.GetService<AppDbContext>();

//            var role = new Role { Name = "Admin", Permissions = new List<string> { } };
//            await roleManager.CreateAsync(role);

//            await userManager.AddToRoleAsync(user, role.Name);
//            await appDbContext.SaveChangesAsync();

//            testRef.GetRef = () => new Session(user.Id, user.FirstName, user.Email, role.Name, role.Id);
//            var service = collection.GetService<ICustomerService>()!;
//            var customer1 = new CreateCustomerPayload("Innovantics", "1234565434", "TR51232", "+2349084453844", "jamesdoe@mailinator.com", "9898767623", "Lagos");
//            var customer2 = new CreateCustomerPayload("MobileTee", "1904565434", "TR50032", "+2349084453812", "mobiletee@mailinator.com", "9090567623", "Lagos");

//            var register1 = service.RegisterAsync(customer1).Result;
//            var register2 = service.RegisterAsync(customer2).Result;

//            var filter = new CustomerFilterPayload("1904565434", "", "");
//            var result = service.GetAllCustomers(filter).GetAwaiter().GetResult();
//            Assert.Equal(1, result.Data?.Total);
//        }

//        [Fact]
//        public async void GetCustomers_InvalidPayload_ReturnAll()
//        {
//            TestRef<Session> testRef = new TestRef<Session>();

//            var collection = GetCollection().AddTransient(xy =>
//            {
//                var moqObj = new Mock<ISessionService>();
//                moqObj.Setup(x => x.HasPermission(It.IsAny<string>())).Returns<string>(c => !string.IsNullOrEmpty(c));
//                moqObj.Setup(x => x.GetSession()).Returns(testRef.GetRef);
//                return moqObj.Object;
//            }).BuildServiceProvider();

//            var userManager = collection.GetService<UserManager<Persona>>();
//            var user = new Persona("jethro.daniel@innovantics.com", "Jethro", "Daniel");
//            var xy = await userManager.CreateAsync(user, "Password123#");

//            var roleManager = collection.GetService<RoleManager<Role>>();
//            var appDbContext = collection.GetService<AppDbContext>();

//            var role = new Role { Name = "Admin", Permissions = new List<string> { } };
//            await roleManager.CreateAsync(role);

//            await userManager.AddToRoleAsync(user, role.Name);
//            await appDbContext.SaveChangesAsync();

//            testRef.GetRef = () => new Session(user.Id, user.FirstName, user.Email, role.Name, role.Id);
//            var service = collection.GetService<ICustomerService>();
//            var customer1 = new CreateCustomerPayload("Innovantics", "1234565434", "TR51232", "+2349084453844", "jamesdoe@mailinator.com", "9898767623", "Lagos");
//            var register1 = service.RegisterAsync(customer1).Result;

//            var customer2 = new CreateCustomerPayload("MobileTee", "1904565434", "TR50032", "+2349084453812", "mobiletee@mailinator.com", "9090567623", "Lagos");
//            var register2 = service.RegisterAsync(customer2).Result;

//            var filter = new CustomerFilterPayload(null, null, null);
//            var result = service.GetAllCustomers(filter).Result;
//            Assert.Equal(2, result.Data?.Total);
//        }

//        [Fact]
//        public async void GetCustomers_SimilarNames_ReturnAll()
//        {
//            TestRef<Session> testRef = new TestRef<Session>();

//            var collection = GetCollection().AddTransient(xy =>
//            {
//                var moqObj = new Mock<ISessionService>();
//                moqObj.Setup(x => x.HasPermission(It.IsAny<string>())).Returns<string>(c => !string.IsNullOrEmpty(c));
//                moqObj.Setup(x => x.GetSession()).Returns(testRef.GetRef);
//                return moqObj.Object;
//            }).BuildServiceProvider();

//            var userManager = collection.GetService<UserManager<Persona>>();
//            var user = new Persona("jethro.daniel@innovantics.com", "Jethro", "Daniel");
//            var xy = await userManager.CreateAsync(user, "Password123#");

//            var roleManager = collection.GetService<RoleManager<Role>>();
//            var appDbContext = collection.GetService<AppDbContext>();

//            var role = new Role { Name = "Admin", Permissions = new List<string> { } };
//            await roleManager.CreateAsync(role);

//            await userManager.AddToRoleAsync(user, role.Name);
//            await appDbContext.SaveChangesAsync();

//            testRef.GetRef = () => new Session(user.Id, user.FirstName, user.Email, role.Name, role.Id);
//            var service = collection.GetService<ICustomerService>();
//            var customer1 = new CreateCustomerPayload("MobileLee", "1234565434", "TR51232", "+2349084453844", "jamesdoe@mailinator.com", "9898767623", "Lagos");
//            var register1 = service.RegisterAsync(customer1).Result;

//            var customer2 = new CreateCustomerPayload("MobileTee", "1904565434", "TR50032", "+2349084453812", "mobiletee@mailinator.com", "9090567623", "Lagos");
//            var register2 = service.RegisterAsync(customer2).Result;

//            var filter = new CustomerFilterPayload(null, null, "mobile");
//            var result = service.GetAllCustomers(filter).Result;
//            Assert.Equal(2, result.Data?.Total);
//        }

//        #endregion
//    }
//}