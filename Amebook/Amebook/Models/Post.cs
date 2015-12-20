using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Amebook.Models
{
    public class Post
    {
        public string PostId { get; set; }
        public string OrginId { get; set; }
        public byte[] Content { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public byte[] Key { get; set; }
        public bool Rated { get; set; }
        public int Plus { get; set; }
        public int Minus { get; set; }
        public string AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        


    }
}