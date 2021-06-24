using Microsoft.AspNetCore.Identity;
using System;

namespace Api.Data.Entities.Account
{
    public class Persona : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Created { get; init; }

        public Persona(string email, string firstName, string lastName) : base(email)
        {
            Created = DateTime.Now;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}