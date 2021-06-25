using Api.Data.Repositories;

namespace Api.Data
{
    public interface IUnitOfWork
    {       
        IRoleRepository RoleRepository { get; }
        IVideoRepository VideoRepository { get; }
        ISearchHistoryRepository SearchHistoryRepository { get; }
    }
}