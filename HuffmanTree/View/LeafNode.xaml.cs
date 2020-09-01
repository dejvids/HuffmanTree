using HuffmanTree.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HuffmanTree.View
{
    /// <summary>
    /// Interaction logic for LeafNode.xaml
    /// </summary>
    public partial class LeafNode : UserControl
    {
        private int frequency;
        private char symbol;

        public LeafNode()
        {
            InitializeComponent();
        }

        public LeafNode(int frequency, char symbol)
        {
            CharToSymbolConverter converter = new CharToSymbolConverter();
            CharacterToFontSizeConverter fontsizeConverter = new CharacterToFontSizeConverter();
            InitializeComponent();
            this.frequency = frequency;
            this.symbol = symbol;
            freqLbl.Text = frequency.ToString();
            signLbs.Text = converter.Convert(symbol, typeof(string),null,Thread.CurrentThread.CurrentCulture).ToString();
            signLbs.FontSize = Convert.ToDouble(fontsizeConverter.Convert(symbol, typeof(double), signLbs.FontSize, Thread.CurrentThread.CurrentCulture));
        }
    }
}
