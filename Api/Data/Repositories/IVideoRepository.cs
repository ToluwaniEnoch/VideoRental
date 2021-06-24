using Api.Data.Entities.Parties;
using Api.Models.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data.Repositories
{
    public interface IVideoRepository : IGenericRepository<Video>
    {
        ValueTask<bool> VideoExists(string Title);
        IQueryable<Video> GetQuery();
        Task<Video> GetByTitle(string Title);
    }
}
