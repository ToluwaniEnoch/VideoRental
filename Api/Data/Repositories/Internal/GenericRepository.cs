using Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Data.Repositories.Internal
{
    internal abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ILogger<TEntity> _logger;
        private readonly DbSet<TEntity> _dbTable;
        public readonly AppDbContext _dbContext;

        protected GenericRepository(AppDbContext dbContext, ILogger<TEntity> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _dbTable = dbContext.Set<TEntity>();
        }

        /// <summary>
        /// Get Item by ID with tracking
        /// </summary>
        /// <remarks>This return item even if its flag as deleted</remarks>
        /// <param name="id"></param>
        /// <returns>TEntity</returns>
        public async ValueTask<TEntity?> FindByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbTable.FindAsync(id, ct);
        }

        /// <summary>
        /// Get Item by ID with not tracking
        /// </summary>
        /// <remarks>Return null if the item flag isdeleted </remarks>
        /// <param name="id"></param>
        /// <returns>Return TEntity or null</returns>
        public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbTable.AsNoTracking().FirstOrDefaultAsync(a=> !a.IsDeleted && a.Id == id, ct);
        }

        /// <summary>
        /// Create new single item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Return TEntity or null if unable to save</returns>
        public async ValueTask<TEntity?> CreateAsync(TEntity item, CancellationToken ct = default)
        {
            await _dbTable.AddAsync(item);
            if (await _dbContext.TrySaveChangesAsync(_logger, ct))
            {
                return item;
            }
            return null;
        }
        /// <summary>
        /// Create new multiple items
        /// </summary>
        /// <param name="items"></param>
        /// <returns>Return IEnumerable<TEntity> or null if unable to save</returns>
        public async Task<IEnumerable<TEntity>?> CreateAsync(IEnumerable<TEntity> items, CancellationToken ct = default)
        {
            await _dbTable.AddRangeAsync(items, ct);
            if (await _dbContext.TrySaveChangesAsync(_logger, ct))
              return items;
            return null;
        }
        /// <summary>
        /// Update item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns true if successful and false if not</returns>
        public async Task<bool> UpdateAsync(TEntity item, CancellationToken ct = default)
        {
            _dbContext.Entry(item).State = EntityState.Modified;
            if (await _dbContext.TrySaveChangesAsync(_logger, ct))
                return true;
            return false;
        }

        public IQueryable<TEntity> GetQuery() => _dbTable;
    }
}