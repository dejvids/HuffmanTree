using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HuffmanTree.Converters
{
    public class BitArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string resutl = string.Empty;
            BitArray bits = value as BitArray;
            if(bits != null)
            {
                foreach(bool bit in bits)
                {
                    resutl += bit ? '1' : '0';
                }
            }
            return resutl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string encoded = value as string;
            bool[] bitsArray = new bool[encoded.Length];
            for(int i = 0; i< encoded.Length; i++)
            {
                if (encoded[i].Equals('1'))
                    bitsArray[i] = true;
                else
                    bitsArray[i] = false;
            }
            BitArray bits = new BitArray(bitsArray);
            return bits;
        }
    }
}
