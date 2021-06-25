using Api.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace Api.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRoleRepository RoleRepository { get; init; }
        public IVideoRepository VideoRepository { get; init; }
        public ISearchHistoryRepository SearchHistoryRepository { get; init; }
        //public IAccountRepository AccountRepository { get; init; }

        public UnitOfWork(IRoleRepository roleRepository, IVideoRepository videoRepository, ISearchHistoryRepository searchHistoryRepository)
        {
            RoleRepository = roleRepository;
            VideoRepository = videoRepository;
            SearchHistoryRepository = searchHistoryRepository;
            //AccountRepository = accountRepository;
        }
    }
}