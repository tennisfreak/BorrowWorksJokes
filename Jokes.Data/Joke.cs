using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Jokes.Entities
{
    public class Joke : BaseEntity
    {
        public int JokeType_ID { get; set; }
        public string Text { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }

        [ForeignKey("JokeType_ID")]
        public JokeType Type { get; set; }
    }
}
