using Api.Data.Entities.Account;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Data.Repositories
{
    public interface IRoleRepository
    {
        Task<IdentityResult> CreateRoleAsync(Role role, CancellationToken ct = default);

        Task<Role> GetUserRoleAsync(Guid personaId, CancellationToken ct = default);

        IQueryable<Role> GetRolesQuery();
    }
}