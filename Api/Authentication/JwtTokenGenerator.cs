using Api.Models.Constants;
using Api.Models.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Authentication
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<JwtTokenGenerator> jwtLogger;
        /// <summary>
        /// Public Constructor for JwtTokenGenerator class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="jwtLogger"></param>
        public JwtTokenGenerator(IConfiguration configuration, ILogger<JwtTokenGenerator> jwtLogger)
        {
            this.configuration = configuration;
            this.jwtLogger = jwtLogger;
        }

        /// <summary>
        /// Generate JWT Token
        /// </summary>
        /// <param name="session"></param>
        /// <param name="expiryTime"></param>
        /// <returns></returns>
        public string GenerateToken(Session session, TimeSpan expiryTime)
        {
            var keyParam = configuration.GetValue(StringConstants.JwtKey, StringConstants.JwtKeyDefault);
            if (keyParam == StringConstants.JwtKeyDefault)
            {
                jwtLogger.LogWarning("jwt is using a default value, please change");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(keyParam);
            var userIdentity = new ClaimsIdentity(GetClaims(session));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim("Id", session.PersonaId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, session.Email),
                    new Claim(nameof(Session.Email), session.Email),
                    new Claim(nameof(Session.Name), session.Name),
                    new Claim(nameof(Session.RoleId), session.RoleId.ToString()),
                    new Claim(nameof(Session.Role), session.Role),
                }),
                Expires = DateTime.Now.Add(expiryTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private List<Claim> GetClaims(Session session) => new List<Claim>
        {
            new Claim(nameof(Session.Email), session.Email),
            new Claim(nameof(Session.Name), session.Name),
            new Claim(nameof(Session.PersonaId), session.PersonaId.ToString() ?? ""),
            new Claim(nameof(Session.RoleId), session.RoleId.ToString() ?? ""),
            new Claim(nameof(Session.Role), session.Role),

        };
    }
}