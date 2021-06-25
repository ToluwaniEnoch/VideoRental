using Api.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace Api.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public IVideoRepository VideoRepository { get; init; }
        public ISearchHistoryRepository SearchHistoryRepository { get; init; }

        public UnitOfWork(IVideoRepository videoRepository, ISearchHistoryRepository searchHistoryRepository)
        {
            VideoRepository = videoRepository;
            SearchHistoryRepository = searchHistoryRepository;
        }
    }
}