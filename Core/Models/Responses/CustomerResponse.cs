using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Responses
{
    public record CustomerResponse(Guid CustomerId, string CompanyName, string TIN, string RcNumber, string CustomerPhoneNumber, string CustomerEmail, string AccountNumber );
            
}
