using Api.Data.Entities.Account;
using Api.Models.Constants;
using Api.Models.Internals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Api.Services.Auth
{
    public class SessionService : ISessionService
    {
        private static readonly object claimsLocker = new object();
        private static readonly object locker = new object();
        private readonly IDistributedCache cache;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly ILogger<SessionService> logger;
        private readonly RoleManager<Role> roleManager;

        private Dictionary<string, Claim>? claimsDictionary;

        private Session? instanceSession;

        private Dictionary<string, string>? permissionDictionary;

        public SessionService(ILogger<SessionService> logger, IHttpContextAccessor contextAccessor, IDistributedCache cache, RoleManager<Role> roleManager)
        {
            this.logger = logger;
            this.contextAccessor = contextAccessor;
            this.cache = cache;
            this.roleManager = roleManager;
        }

        public Session? GetSession()
        {
            try
            {
                if (instanceSession is null)
                {
                    instanceSession = ProcessSession();
                    logger.LogInformation($"Resolved New Session For {instanceSession?.Email ?? instanceSession?.Name}");
                }

                //todo find a way to prevent the session from being overwritten
                return instanceSession;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to resolve session");
                return null;
            }
        }

        public bool HasPermission(string permission)
        {
            GetSession();
            ProcessPermissions();
            if (instanceSession?.Role == StringConstants.SUPER_ADMIN) return true;
            if (permissionDictionary is null) return false;

            return permissionDictionary.ContainsKey(permission.ToLower());
        }

        private List<string>? GetPermissions(Guid? roleId)
        {
            if (roleId == null)
                return Enumerable.Empty<string>().ToList();
            var roleJsonStr = cache.GetString($"{instanceSession?.Role}_ROLE");
            if (!string.IsNullOrEmpty(roleJsonStr))
                return JsonSerializer.Deserialize<List<string>>(roleJsonStr);

            var roleItem = roleManager.Roles.FirstOrDefault(x => x.Id == roleId);
            if (roleItem is null)
                return Enumerable.Empty<string>().ToList();
            cache.SetString($"{instanceSession?.RoleId}_ROLE", JsonSerializer.Serialize(roleItem.Permissions), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3) });
            return roleItem.Permissions;
        }

        private Dictionary<string, Claim> GetClaimsDict()
        {
            var claims = contextAccessor.HttpContext?.User.Claims;
            var _claimsDictionary = new Dictionary<string, Claim>();
            if (claims is not null)
            {
                foreach (var claim in claims)
                {
                    lock (claimsLocker)
                    {
                        _claimsDictionary[claim.Type] = claim;
                    }
                }
            }
            return _claimsDictionary;
        }

        private void ProcessPermissions()
        {
            if (permissionDictionary is { }) return;
            GetSession(); //incase session has not being created
            if (instanceSession is null) return;

            var permissions = GetPermissions(instanceSession.RoleId);
            if (permissions == null || !permissions.Any()) return;
            permissionDictionary = new Dictionary<string, string>();

            foreach (var perm in permissions)
            {
                lock (locker)
                {
                    permissionDictionary[perm.ToLower()] = perm;
                }
            }
        }

        private Session? ProcessSession()
        {
            if (contextAccessor.HttpContext?.User is null) return null;
            if (claimsDictionary == null) 
                claimsDictionary = GetClaimsDict();
            
            claimsDictionary.TryGetValue(nameof(Session.Email), out var emailClaim);
            var email = emailClaim?.Value;
            if (string.IsNullOrEmpty(email)) email = null;

            claimsDictionary.TryGetValue(nameof(Session.RoleId), out var roleIdClaim);
            Guid.TryParse(roleIdClaim?.Value, out var roleId);

            claimsDictionary.TryGetValue(nameof(Session.Name), out var nameClaim);
            var name = nameClaim?.Value;
            if (string.IsNullOrEmpty(name)) name = null;

            claimsDictionary.TryGetValue(nameof(Session.Role), out var roleNameClaim);
            var role = roleNameClaim?.Value;
            if (string.IsNullOrEmpty(role)) role = null;

            claimsDictionary.TryGetValue("Id", out var personaClaim);
            Guid.TryParse(personaClaim?.Value, out var personId);

            return new Session(personId, name ?? string.Empty, email ?? string.Empty, role ?? string.Empty, roleId);
        }
    }
}