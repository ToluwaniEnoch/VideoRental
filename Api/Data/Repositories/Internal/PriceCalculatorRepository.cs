using Api.Data.Entities.Parties;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data.Repositories.Internal
{
    internal class PriceCalculatorRepository : GenericRepository<Price>, IPriceCalculatorRepository
    {
        private readonly AppDbContext _dbContext;

        public PriceCalculatorRepository(AppDbContext dbContext, ILogger<Price> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
        }
    }
}
