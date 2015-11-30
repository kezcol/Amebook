using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Amebook.Models
{
    public class Account
    {

        public string AccountId { get; set; }
        public string Nickname { get; set; }


        [ForeignKey("AccountId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
    }
}