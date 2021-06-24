using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Responses
{
    public record CustomerResponse(string FirstName, string LastName, string Email, Guid CustomerId);

}
