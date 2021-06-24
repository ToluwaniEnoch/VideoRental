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

        
    }
}
