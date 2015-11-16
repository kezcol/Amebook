using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Amebook.Models
{
    public class Invitation
    {
        public String HashCode { get; set; }
        public DateTime CreationDate { get; set; }
        public int DurationTime = 15 * 60;
        public String AccountID { get; set; }
        public String InvitationKey { get; set; }

        public Invitation()
        {
            InvitationKey = Amebook.Crypto.CryptoRandom.GenerateKey(30);
        }
    }
}