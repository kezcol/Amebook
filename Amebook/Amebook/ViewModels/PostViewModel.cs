using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amebook.ViewModels
{
    public class PostViewModel
    {
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public string Id { get; set; }
        public int Plus { get; set; }
        public int Minus { get; set; }
        public bool Rated { get; set; }
    }
}