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
        public byte[] PublicKey { get; set; }
        public byte[] Exponent { get; set; }
        public byte[] _Modulus { get; set; }
        public byte[] _D { get; set; }
        public byte[] _DP { get; set; }
        public byte[] _Exponent { get; set; }
        public byte[] _DQ { get; set; }
        public byte[] _InvereQ { get; set; }
        public byte[] _P { get; set; }
        public byte[] _Q { get; set; }

        [ForeignKey("AccountId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<Post> Posts { get; set; }

        public Account()
        {
            var provider = new RSACryptoServiceProvider();
            RSAParameters RSAKeyInfo = provider.ExportParameters(true);
            _Modulus = RSAKeyInfo.Modulus;
            _D = RSAKeyInfo.D;
            _DP = RSAKeyInfo.DP;
            _Exponent = RSAKeyInfo.Exponent;
            _DQ = RSAKeyInfo.DQ;
            _InvereQ = RSAKeyInfo.InverseQ;
            _P = RSAKeyInfo.P;
            _Q = RSAKeyInfo.Q;

            RSAKeyInfo = provider.ExportParameters(false);
            PublicKey = RSAKeyInfo.Modulus;
            Exponent = RSAKeyInfo.Exponent;

        }

    }
}