using Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Data.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        ValueTask<TEntity?> FindByIdAsync(Guid id, CancellationToken ct = default);

        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);

        ValueTask<TEntity?> CreateAsync(TEntity item, CancellationToken ct = default);

        Task<IEnumerable<TEntity>?> CreateAsync(IEnumerable<TEntity> items, CancellationToken ct = default);

        Task<bool> UpdateAsync(TEntity item, CancellationToken ct = default);

        IQueryable<TEntity> GetQuery();
    }
}