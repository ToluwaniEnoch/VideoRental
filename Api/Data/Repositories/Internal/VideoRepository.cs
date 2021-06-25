using Api.Data.Entities.Parties;
using Api.Models.Payloads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data.Repositories.Internal
{
    internal class VideoRepository : GenericRepository<Video>, IVideoRepository
    {
        private readonly AppDbContext _dbContext;

        public VideoRepository(AppDbContext dbContext, ILogger<Video> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
        }

        public Task<Video> GetByTitle(string title)
        {
            var query = from video in _dbContext.Videos
                        select video;

            if (title is { } && title.Any())
                query = query.Where(x => title.ToLower().Equals(x.Title.ToLower()));

            return query.FirstOrDefaultAsync();
        }

        public IQueryable<Video> GetQuery()
        {
            var query = from video in _dbContext.Videos
                        select video;

           // if (payload.Title is { } && payload.Title.Any())               query = query.Where(x => payload.Title.Contains(x.Title));



            return query.OrderByDescending(x => x.Created);
        }

        public async ValueTask<bool> VideoExists(string Title)
        {
            var videoExists = await _dbContext.Videos.AnyAsync(x => !x.IsDeleted && x.Title.ToLower().Contains(Title.ToLower()));
            return videoExists;
        }
    }
}
