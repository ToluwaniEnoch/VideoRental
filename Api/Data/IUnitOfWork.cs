using Api.Data.Repositories;

namespace Api.Data
{
    public interface IUnitOfWork
    {       
        IVideoRepository VideoRepository { get; }
        ISearchHistoryRepository SearchHistoryRepository { get; }
    }
}