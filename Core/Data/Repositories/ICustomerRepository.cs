using Api.Data.Entities.Parties;
using Api.Models.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        ValueTask<bool> CustomerExists(string TIN, string RcNUmber);
        IQueryable<Customer> GetQuery(CustomerFilterPayload payload);
    }
}
