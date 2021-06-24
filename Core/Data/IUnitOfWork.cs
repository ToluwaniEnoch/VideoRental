using Api.Data.Repositories;

namespace Api.Data
{
    public interface IUnitOfWork
    {       
        ICustomerRepository CustomerRepository { get; }
        IRoleRepository RoleRepository { get; }
    }
}