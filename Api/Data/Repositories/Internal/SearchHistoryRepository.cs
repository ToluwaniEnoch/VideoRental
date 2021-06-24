using Api.Data.Entities.Parties;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data.Repositories.Internal
{
    internal class SearchHistoryRepository : GenericRepository<SearchHistory>, ISearchHistoryRepository
    {
        private readonly AppDbContext _dbContext;

        public SearchHistoryRepository(AppDbContext dbContext, ILogger<SearchHistory> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
        }
    }
}
