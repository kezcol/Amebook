using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Amebook.Models
{
    public class Friend
    {
        public int Id { get; set; }

        public string AccountId { get; set; }
        public string FriendId { get; set; }
        public DateTime ConnectionDate { get; set; }

        [ForeignKey("FriendId")]
        public virtual Account Invitated { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }


    }
}