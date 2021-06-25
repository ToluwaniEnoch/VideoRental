using Api.Models.Enums;
using System;
using System.Collections.Generic;
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
    
    public class ChildrenMovie
    {
        public string Title { get; set; }
        public int MaximumAge { get; set; }
        public Guid Id { get; set; }
    }
    public class NewReleaseMovie
    {
        public string Title { get; set; }
        public int YearReleased { get; set; }
        public Guid Id { get; set; }
    }
    
}
