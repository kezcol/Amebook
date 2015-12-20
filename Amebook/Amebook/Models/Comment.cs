using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amebook.Models
{
    public class Comment
    {
        public string CommentId { get; set; }
        public string OrginId { get; set; }
        public byte[] Content { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public byte[] Key { get; set; }
    }
}