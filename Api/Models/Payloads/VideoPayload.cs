using Api.Models.Enums;
using Api.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Payloads
{
    public record CreateVideoPayload 
    { 
        [Required]
        public string Title { get; init; }

        [Required, EnumDataType(typeof(VideoType))]
        public VideoType Type { get; init; }

        [Required, EnumDataType(typeof(VideoGenre))]
        public VideoGenre Genre { get; init; }

        [RequiredIf(nameof(Type), VideoType.ChildrenMovie, ErrorMessage = "Please Provide the maximum age for the movie"), Range(1, 150)]
        public int? MaximumAge { get; init; }

        [RequiredIf(nameof(Type), VideoType.NewRelease, ErrorMessage = "Please Provide the Year released for the movie"),  NewRelease]
        public int? YearReleased { get; init; }
    }
    
    public record VideoFilterPayload(string? Title, VideoType? Type, VideoGenre? Genre);
}
