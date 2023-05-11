using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using Microsoft.Win32;
using My.GIS;
using System.Runtime.Remoting.Channels;

namespace Wpf8F
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Layer _layer;
        public Rectangle ClientRectangle = new Rectangle();
        MapAndClientConverter _converter = null;
        public MainWindow()
        {
            InitializeComponent();
            ClientRectangle.Height = this.MapCavans.ActualHeight;
            ClientRectangle.Width = this.MapCavans.ActualWidth;
            _converter = new MapAndClientConverter(new GISMapExtent(new GISVertex(0, 0), new GISVertex(100, 100)), ClientRectangle);
        }
        private void UpdateClientSize()
        {
            ClientRectangle.Width = this.MapCavans.ActualWidth;
            ClientRectangle.Height = this.MapCavans.ActualHeight;
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); openFileDialog.Filter = "Shapefile file |*.shp";
            openFileDialog.RestoreDirectory = false;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            openFileDialog.ShowDialog();
            ShapefileTools shapefileTools = new ShapefileTools();
            _layer = shapefileTools.ReadShapefile(openFileDialog.FileName);
            _layer.DrawAttributeOrNot = false;
            MessageBox.Show("Read " + _layer.FeatureCount() + " objects");

            UpdateAndDraw(_layer.Extent, ClientRectangle);
            shape_box.Text = _layer.ShapeType.ToString();
            x_extent_box.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _layer.Extent.minX(), _layer.Extent.maxX());
            y_extent_box.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _layer.Extent.minY(), _layer.Extent.maxY());

        }
        private void UpdateAndDraw(GISMapExtent extent, Rectangle clientRectangle)
        {
            _converter.UpdateConverter(extent, clientRectangle);
            MapCavans.Children.Clear();
            Size_Box.Text = clientRectangle.Width.ToString() + " " + clientRectangle.Height.ToString();
            DrawMap();

        }
        private void DrawMap()
        {
            _layer.Draw(MapCavans, _converter);

        }
        private Rect GetBoundingBox(FrameworkElement element, Window containerWindow)
        {
            GeneralTransform transform = element.TransformToAncestor(containerWindow);
            Point topLeft = transform.Transform(new Point(0, 0));
            Point bottomRight = transform.Transform(new Point(element.ActualWidth, element.ActualHeight));
            return new Rect(topLeft, bottomRight);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateClientSize();
            if (_layer != null)
            {
                UpdateAndDraw(_layer.Extent, ClientRectangle);
            }
        }

        private void MenuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Save_qu_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.HasValue) return;
            string fileName = dialog.FileName;
            MyFiles.WriteFile(_layer, fileName);
            MessageBox.Show("done");

        }

        private void Attributes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AttributeWindow atttibuteWindow = new AttributeWindow(_layer);
                atttibuteWindow.Title = "Attributes table";
                atttibuteWindow.Show();

            }
            catch (Exception ex)
            {

                MessageBox.Show("please open a file");
            }
        }

        private void Read_qu_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            string fileName = dialog.FileName;
            _layer = MyFiles.ReadFile(fileName);
            MessageBox.Show("Read " + _layer.FeatureCount() + " objects");
            _converter.UpdateConverter(_layer.Extent, ClientRectangle);
            DrawMap();
        }

    }
}
