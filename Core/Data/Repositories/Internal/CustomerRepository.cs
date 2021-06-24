using Api.Data.Entities.Parties;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Api.Models.Payloads;

namespace Api.Data.Repositories.Internal
{
    internal class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext dbContext;

        public CustomerRepository(AppDbContext dbContext, ILogger<Customer> logger) : base(dbContext, logger)
        {
            this.dbContext = dbContext;
        }

        public async ValueTask<bool> CustomerExists(string TIN, string RcNumber)
        {
            var customerExists = await dbContext.Customers.AnyAsync(x => !x.IsDeleted && x.TIN.Contains(TIN) && x.RcNumber.Contains(RcNumber));
            return customerExists;
        }

        public IQueryable<Customer> GetQuery(CustomerFilterPayload payload)
        {
            var query = from user in dbContext.Customers
                        select user;

            if (payload.TIN is { } && payload.TIN.Any())
                query = query.Where(x => payload.TIN.Contains(x.TIN));

            if (payload.RcNumber is { } && payload.RcNumber.Any())
                query = query.Where(x => payload.RcNumber.Contains(x.RcNumber));

            if (!string.IsNullOrEmpty(payload.CompanyName))
                query = query.Where(x => (x.CompanyName).ToLower().Contains(payload.CompanyName.ToLower()));

            return query.OrderByDescending(x => x.Created);
        }

        
    }
}
