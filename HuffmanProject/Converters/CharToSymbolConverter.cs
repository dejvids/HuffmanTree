using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HuffmanTree.Converters
{
    public class CharToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            char sign = (char)value;
            if(sign.Equals(' '))
            {
                return "spacja";
            }
            if (sign.Equals('\n'))
                return "Nowa\n linia";
            if (sign.Equals('\r'))
                return "Powrót\r karetki";
            return sign;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
