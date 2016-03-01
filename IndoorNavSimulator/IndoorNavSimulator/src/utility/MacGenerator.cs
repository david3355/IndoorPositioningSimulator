using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndoorNavSimulator
{
    class MacGenerator
    {
        private static Random rnd = new Random();
        private static char[] tags = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        public static String GenerateRandomMAC()
        {
            int n = 12;
            StringBuilder stb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                stb.Append(tags[rnd.Next(tags.Length)]);
                if (i % 2 == 1 && i != n - 1) stb.Append(":");
            }
            return stb.ToString();
        }
    }
}
