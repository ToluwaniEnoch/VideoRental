using Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Responses
{
    public record VideoResponse(Guid Id, string Title, VideoType Type, VideoGenre Genre);

}
