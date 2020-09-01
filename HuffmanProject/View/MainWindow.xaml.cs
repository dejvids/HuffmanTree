using HuffmanTree.ViewModel;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HuffmanTree.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double nodeWidth;
        double nodeHeight;
        double diagramWidth;
        int margin;
        int verticalMargin;

        public MainWindow()
        {
            InitializeComponent();
            this.ViewModel.DrawCommand.Subscribe(x =>
            {
                DrawDiagram();
            });
        }

        private void DrawDiagram()
        {
            verticalMargin = 80;
            int treeHeight = ViewModel.TreeHight;
            int maxNodes = (int)Math.Pow(2, (double)treeHeight);
            diagramWidth = (nodeWidth + margin) * maxNodes;
            int diagramHeight = verticalMargin * treeHeight;
            //zwiekszenie szerokosci kanwy o 10%
            diagramWidth *= 1.1;
            treeView.Width = diagramWidth < mainWindow.ActualWidth ? mainWindow.ActualWidth : diagramWidth;
            treeView.Height = diagramHeight < mainWindow.Height ? mainWindow.Height : diagramHeight;
            nodeWidth = 50;
            nodeHeight = 50;
            margin = 0;

            Node rootNode = ViewModel.Nodes.FirstOrDefault();
            double x = treeView.Width / 2;
            double y = 10;
            NodeCircle root = new NodeCircle(rootNode.Frequency);
            treeView.Children.Clear();
            treeView.Children.Add(root);
            root.SetValue(Canvas.LeftProperty, x);
            root.SetValue(Canvas.TopProperty, y);
            double distance = diagramWidth / 2;
            drawTree(rootNode, x, y, distance, x, y,1);
        }


        private void InputTex_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputText.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }


        private void drawTree(Node node, double x, double y, double distance, double x2, double y2,int sign)
        {
            distance = distance / 2;
            int labelSign = sign;
            if (node.Right != null)
            {
                drawTree(node.Right, x + distance+ margin, y + verticalMargin,distance, x, y,1);
            }

            if (node.Left != null)
            {
                drawTree(node.Left, x - distance - margin, y + verticalMargin, distance, x, y,0);
            }


            if (node.IsLeaf)
            {
                LeafNode leaf = new LeafNode(node.Frequency, node.Symbol);
                treeView.Children.Add(leaf);
                leaf.SetValue(Canvas.LeftProperty, x);
                leaf.SetValue(Canvas.TopProperty, y);
                leaf.SetValue(Canvas.ZIndexProperty, 2);
            }
            else
            {
                NodeCircle nodeCircle = new NodeCircle(node.Frequency);
                treeView.Children.Add(nodeCircle);
                nodeCircle.SetValue(Canvas.LeftProperty, x);
                nodeCircle.SetValue(Canvas.TopProperty, y);
                nodeCircle.SetValue(Canvas.ZIndexProperty, 2);
            }
            if (x != x2)
            {
                Line line = new Line() { X1 = x + nodeWidth/2, Y1 = y+nodeHeight/2, X2 = x2 + nodeWidth/2, Y2 = y2 + nodeHeight/2, StrokeThickness = 2, Stroke = new SolidColorBrush(Colors.Black)};
                Label label = new Label() { Content = labelSign.ToString(),FontSize=14 };
                treeView.Children.Add(line);
                treeView.Children.Add(label);
                if(sign == 1)
                    label.SetValue(Canvas.LeftProperty, x2 + (x - x2)*0.5 + nodeWidth/2);
                else
                    label.SetValue(Canvas.LeftProperty, x2 - (x2 - x) * 0.5);

                label.SetValue(Canvas.TopProperty, y - verticalMargin * 0.5);
            }


        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveImage(this,treeView ,96);
        }

        public void SaveImage(Window window, Canvas canvas, int dpi)
        {
            Size size = new Size(window.Width, window.Height);
            canvas.Measure(size);
            //canvas.Arrange(new Rect(size));
            int width = (int)canvas.Width;
             if(window.Width > canvas.Width)
            {
                width = (int)window.Width;
            }
            var rtb = new RenderTargetBitmap(
                width, //width
                (int)canvas.ActualHeight + 10, //height
                dpi, //dpi x
                dpi, //dpi y
                PixelFormats.Pbgra32 // pixelformat
                );
            rtb.Render(canvas);
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Png Image | *.png";
            dialog.DefaultExt = "png";
            if (dialog.ShowDialog() == true)
            {
                SaveRTBAsPNG(rtb, dialog.FileName);
            }
        }
        private  void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
        {
            var enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DigaramItem.IsSelected)
                if (ViewModel.IsEncoded == true)
                    ViewModel.DrawCommand.Execute().Subscribe();
        }

        private void ChartersData_Loaded(object sender, RoutedEventArgs e)
        {
            CharactersData.Items.Refresh();
        }

        private void EncodedBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void EncodedBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[0,1]"); 
            return regex.IsMatch(text);
        }

        private void EncodedBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EncodedBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //treeView.Width = mainWindow.Width;
        }
    }
}
