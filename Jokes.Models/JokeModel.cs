using System;
using System.ComponentModel.DataAnnotations;

namespace Jokes.Models
{
    public class JokeModel
    {
        [Required]
        public Guid ID { get; set; }
        [Required]
        public JokeTypeModel Type { get; set; }
        //[Required]
        //public int JokeType_ID { get; }
        [Required]
        [MaxLength(1024)]
        public String Text { get; set; }
        [Required]
        public int LikeCount { get; set; }
        [Required]
        public int DislikeCount { get; set; }
    }
}
