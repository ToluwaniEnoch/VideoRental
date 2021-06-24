using Api.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace Api.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRoleRepository RoleRepository { get; init; }
        public ICustomerRepository CustomerRepository { get; init; }

        public UnitOfWork(IRoleRepository roleRepository, ICustomerRepository customerRepository)
        {
            RoleRepository = roleRepository;
            CustomerRepository = customerRepository;
        }
    }
}