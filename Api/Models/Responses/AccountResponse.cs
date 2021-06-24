using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Responses
{
    public record AccountResponse(string FirstName, string LastName, Guid PersonaId);
}
