using Api.Models.Internals;
using System;

namespace Api.Authentication
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Session session, TimeSpan expiryTime);
    }
}