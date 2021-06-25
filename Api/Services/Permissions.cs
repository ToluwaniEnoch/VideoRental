using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
    public static class Permissions
    {
        private const string Video = "Video/";

        public const string CanViewVideos = Video + nameof(CanViewVideos);
        public const string CanUploadVideos = Video + nameof(CanUploadVideos);


    }
}
