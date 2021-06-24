using Api.Data.Repositories;

namespace Api.Data
{
    public interface IUnitOfWork
    {       
        ICustomerRepository CustomerRepository { get; }
        IRoleRepository RoleRepository { get; }
        IVideoRepository VideoRepository { get; }
        ISearchHistoryRepository SearchHistoryRepository { get; }
        //IAccountRepository AccountRepository { get; }
    }
}