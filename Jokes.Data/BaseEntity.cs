using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jokes.Entities
{
    public class BaseEntity
    {
        [Key]
        public virtual Guid ID { get; set; }
        [Required]
        public virtual Guid ModifiedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }

        [ForeignKey("ModifiedBy")]
        public User User { get; set; }
    }

    public class BaseLookup
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual string Value { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
