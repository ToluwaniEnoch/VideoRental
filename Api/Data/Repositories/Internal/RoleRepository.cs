using Api.Data.Entities.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Data.Repositories.Internal
{
    internal class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly AppDbContext _appDbContext;

        public RoleRepository(RoleManager<Role> roleManager, AppDbContext appDbContext)
        {
            _roleManager = roleManager;
            _appDbContext = appDbContext;
        }

        public async Task<IdentityResult> CreateRoleAsync(Role role, CancellationToken ct = default)
        {
            return await _roleManager.CreateAsync(role);
        }

        public IQueryable<Role> GetRolesQuery()
        {
            return _roleManager.Roles;
        }

        public async Task<Role> GetUserRoleAsync(Guid personaId, CancellationToken ct = default)
        {
            var query = from user in _appDbContext.Users
                        join userRole in _appDbContext.UserRoles on user.Id equals userRole.UserId
                        join role in _appDbContext.Roles on userRole.RoleId equals role.Id
                        where user.Id == personaId
                        select role;

            return await query.FirstOrDefaultAsync(ct);
        }
    }
}