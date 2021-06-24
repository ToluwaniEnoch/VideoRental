using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Payloads
{   

    public record CreateCustomerPayload(string CompanyName, string TIN, string RcNumber, string CustomerPhoneNumber, string CustomerEmail, string AccountNumber, string Address);
    public record CustomerUpdatePayload(string CustomerPhoneNumber, string AccountNumber, string Address, Guid CustomerId);
    public record CustomerFilterPayload(string? TIN, string? RcNumber, string? CompanyName);
    
}
