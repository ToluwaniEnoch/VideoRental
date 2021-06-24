using Api.Models.Enums;
using Api.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Payloads
{
    public record CreateVideoPayload 
    { 
        public string Title { get; init; }
        public VideoType Type { get; init; }
        public VideoGenre Genre { get; init; }

        [RequiredIf(nameof(Type), VideoType.ChildrenMovie, ErrorMessage = "Please Provide the maximum age for the movie")]
        public int MaximumAge { get; init; }

        [RequiredIf(nameof(Type), VideoType.NewRelease, ErrorMessage = "Please Provide the Year released for the movie")]
        public int YearReleased { get; init; }
    }
    
    public record VideoFilterPayload(string? Title, VideoType? Type, VideoGenre? Genre);
}
