using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace Amebook.Models
{
    public class Account
    {

        public string AccountId { get; set; }
        public string Nickname { get; set; }
        public string PublicKey { get; set; }
        [NotMapped]
        public string PrivateKey { get; set; }
            
        [ForeignKey("AccountId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }

        public Account()
        {
            var provider = new RSACryptoServiceProvider();
            PublicKey = provider.ToXmlString(false);
            PrivateKey = provider.ToXmlString(true);

        }

    }
}