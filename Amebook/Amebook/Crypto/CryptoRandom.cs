using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Amebook.Crypto
{
    public class CryptoRandom
    {
        private static RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

        private static int[] generateIntTab(int tab_size, int maxValue)
        {
            byte[] tab = new byte[tab_size];
            provider.GetBytes(tab);
            int[] IntegerArray = tab.Select(x => (int) x).ToArray();
            for (int i = 0; i < IntegerArray.Length; ++i)
            {
                IntegerArray[i] = IntegerArray[i]%maxValue;
            }
            return IntegerArray;

        }

        public static String GenerateKey(int keyLenght)
        {
            string KEY_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
            string KEY_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
            string KEY_CHARS_NUMERIC = "0123456789";
            string KEY_CHARS_SPECIAL = "*$-+?_&=!%@";
            string PASSSTABLE = KEY_CHARS_LCASE + KEY_CHARS_NUMERIC + KEY_CHARS_SPECIAL + KEY_CHARS_UCASE;
            int[] valTab = generateIntTab(keyLenght, PASSSTABLE.Length);
            StringBuilder txt = new StringBuilder();
            char[] passArray = PASSSTABLE.ToArray();
            foreach (var x in valTab)
            {
                txt.Append(passArray[x]);
            }
            return txt.ToString();
        }
    }
}