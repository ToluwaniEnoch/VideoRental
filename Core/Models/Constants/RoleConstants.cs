using Api.Data.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Api.Services.Permissions;

namespace Api.Models.Constants
{
    public static class RoleConstants
    {
        public const string Admin = "Admin";

        public static List<Role> GetDefaultTemplateRoles() => new List<Role>
        {
            new Role
            {
                Name = Admin,

                Permissions = new List<string>
                {
                    CanEditCustomerDetails,
                }
            }
        };
    }
}
