using Api.Data.Entities.Account;
using Api.Data.Entities.Parties;
using Api.Models.Constants;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Data
{
    public class AppDbContext : IdentityDbContext<Persona, Role, Guid>
    {
        private readonly DbContextOptions _dco;

        public AppDbContext(DbContextOptions<AppDbContext> dco) : base(dco)
        {
            _dco = dco;
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<NewReleaseMovie> NewReleaseMovies { get; set; }
        public DbSet<ChildrenMovie> ChildrenMovies { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }

        public async Task<bool> TrySaveChangesAsync(ILogger logger, CancellationToken ct = default)
        {
            try
            {
                await SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateException e)
            {
                logger.LogError($"DB Update Exception Message >>>>> {e.Message}");
                logger.LogError($"DB Update Inner Exception Message >>>>> {e.InnerException?.Message}");
                return false;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_dco is not null)
            {
                base.OnConfiguring(optionsBuilder);
                return;
            }
            optionsBuilder.UseNpgsql(StringConstants.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            if (!Database.IsNpgsql()) //postgres allows to persist array types, for others we can just get a json string
            {
                builder.Entity<Role>().Property(e => e.Permissions).HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { IgnoreNullValues = true }),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions { IgnoreNullValues = true }));
            }
        }
    }
}