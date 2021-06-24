using System;

namespace Api.Models.Internals
{
    public record Session(Guid PersonaId, string Name, string Email, string Role, Guid RoleId);
}