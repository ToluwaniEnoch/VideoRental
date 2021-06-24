using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Data.Entities
{
    public abstract record BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? DeletedAt { get; set; }

        protected BaseEntity()
        {
            IsDeleted = false;
            Created = DateTime.Now;
            Id = Guid.NewGuid();
        }
    }
}