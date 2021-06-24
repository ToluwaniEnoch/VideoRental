using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Payloads
{
    public record CreateAccountPayload(string FirstName, string LastName, string Email, string Password, string ConfirmPassword);
    
    
}
