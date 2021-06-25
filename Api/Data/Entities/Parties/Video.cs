using Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data.Entities.Parties
{
    public class Video : BaseEntity
    {
        public string Title { get; set; }
        public VideoType Type { get; set; }
        public VideoGenre Genre { get; set; }
        public int YearReleased { get; set; }
        public int MaximumAge { get; set; }
    }
    
    public class ChildrenMovie : BaseEntity
    {
        public string Title { get; set; }
        public int MaximumAge { get; set; }

        public Guid VideoId { get; set; }

        [ForeignKey(nameof(VideoId))]
        public Video Video { get; set; }
    }
    public class NewReleaseMovie : BaseEntity
    {
        public string Title { get; set; }
        public int YearReleased { get; set; }
        public Guid VideoId { get; set; }
        [ForeignKey(nameof(VideoId))]
        public Video Video { get; set; }
    }
    
}
