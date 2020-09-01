using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for NodeCircle.xaml
    /// </summary>
    public partial class NodeCircle : UserControl
    {
        public NodeCircle()
        {
            InitializeComponent();
        }
        public NodeCircle(int x, int y)
        {
            InitializeComponent();
            this.SetValue(Canvas.LeftProperty, x);
            this.SetValue(Canvas.TopProperty, y);
        }
        public NodeCircle(int frequency)
        {
            InitializeComponent();
            freqLbl.Text = frequency.ToString();
        }
    }
}
