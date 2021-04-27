using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class Converts
    {
        public static string StringToBinary(string value)
        {
            //MessageBox.Show(value);

            StringBuilder stringBuilder = new StringBuilder();

            var tmp = Encoding.Unicode.GetBytes(value);
            foreach (byte c in tmp)
            {
                stringBuilder.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }

            //MessageBox.Show(stringBuilder.ToString());

            return stringBuilder.ToString();

        }
        public static string BinaryToString(string value)
        {
            List<byte> byteList = new List<byte>();

            try
            {

                for (int i = 0; i < value.Length; i += 8)
                {
                    byteList.Add(Convert.ToByte(value.Substring(i, 8), 2));
                }
            }
            catch
            {
                return string.Empty;
            }

            return Encoding.Unicode.GetString(byteList.ToArray());
        }
    }
}
