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
            int[] IntegerArray = tab.Select(x => (int)x).ToArray();
            for (int i = 0; i < IntegerArray.Length; ++i)
            {
                IntegerArray[i] = IntegerArray[i] % maxValue;
            }
            return IntegerArray;

        }
        private static int next()
        {
            byte[] tab = new byte[4];
            provider.GetBytes(tab);
            return BitConverter.ToInt32(tab, 0);

        }
        public static String GenerateKey(int keyLenght)
        {
            string KEY_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
            string KEY_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
            string KEY_CHARS_NUMERIC = "0123456789";
            string KEY_CHARS_SPECIAL = "*$-+?_&=!%@";
            char[] PASSSTABLE = (KEY_CHARS_LCASE + KEY_CHARS_NUMERIC + KEY_CHARS_SPECIAL + KEY_CHARS_UCASE).ToCharArray();
            int[] valTab = generateIntTab(keyLenght, PASSSTABLE.Length);
            var tmp = valTab.ToList();
            var shuffled = tmp.OrderBy(a => next());
            var shuffledArray = shuffled.ToArray();
            StringBuilder key = new StringBuilder();
            foreach (var x in shuffledArray)
            {
                key.Append(PASSSTABLE[x]);
            }
            return key.ToString();
        }

    }
}