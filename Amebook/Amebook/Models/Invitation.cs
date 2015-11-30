using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web;

namespace Amebook.Models
{
    public class Invitation
    {
        [Key]
        public String InvitationId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public String AccountId { get; set; }
        public String InvitationKey { get; set; }
        public bool Used { get; set; }

        [ForeignKey("AccountId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public Invitation()
        {
            this.InvitationId = Guid.NewGuid().ToString();
            this.ExpirationDate = DateTime.Now.AddMinutes(30);
            this.InvitationKey = Crypto.CryptoRandom.GenerateKey(30);
            this.Used = false;
        }
    }
}