using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
    public static class Permissions
    {
        private const string Customer = "Customer/";

        public const string CanDoAnything = Customer + nameof(CanDoAnything);
        private const string Admin = "Admin/";
        public const string CanUploadVideos = Customer + nameof(CanUploadVideos);


    }
}
