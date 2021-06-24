using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Entities.Account
{
    public class Role : IdentityRole<Guid>
    {
        [Column(TypeName = "jsonb")]
        public List<string>? Permissions { get; set; }
    }
}