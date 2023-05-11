using My.GIS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace lesson9
{
    public partial class Form1 : Form
    {
        Layer _layer = null;
        MapAndClientConverter _converter = null;
        public Form1()
        {
            InitializeComponent();
            _converter = new MapAndClientConverter(new GISMapExtent(new GISVertex(0, 0), new GISVertex(100, 100)), ClientRectangle);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Shapefile file |*.shp";
            openFileDialog.RestoreDirectory = false;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            ShapefileTools shapefileTools = new ShapefileTools();
            _layer = shapefileTools.ReadShapefile(openFileDialog.FileName);
            _layer.DrawAttributeOrNot = false;
            MessageBox.Show("Read " + _layer.FeatureCount() + " objects");
            UpdateAndDraw(_layer.Extent, ClientRectangle);
            shape_box.Text = _layer.ShapeType.ToString();
            x_extent_box.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _layer.Extent.minX(), _layer.Extent.maxX());
            y_extent_box.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _layer.Extent.minY(), _layer.Extent.maxY());

            /*_converter.UpdateConverter(_layer.Extent,ClientRectangle);
            DrawMap();*/

            /*ShapefileTools shapfileTools = new ShapefileTools();
            _layer = shapfileTools.ReadShapefile(@"C:\Users\bre\Downloads\LANDrop\GisData\chap10\thermal.shp");
            string fileName = null;
            _layer.DrawAttributeOrNot = false;
            MessageBox.Show("Fount" + _layer.FeatureCount() + "Points" + fileName);*/

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //update map button clicked
            /*_converter.UpdateConverter(_layer.Extent, ClientRectangle);
            DrawMap();*/
            UpdateAndDraw(_layer.Extent, ClientRectangle);
        }
        private void UpdateAndDraw(GISMapExtent extent, Rectangle clientRectangle)
        {

            _converter.UpdateConverter(extent, clientRectangle);
            DrawMap();
        }


        private void DrawMap()
        {
            Graphics graphics = CreateGraphics();
            graphics.FillRectangle(new SolidBrush(Color.Black), ClientRectangle);

            _layer.Draw(graphics, _converter);
        }
        private void map_button_Click(object sender, EventArgs e)
        {
            GISMapActions mapActions = GISMapActions.zoomin;
            if ((Button)sender == button3)
            {
                mapActions = GISMapActions.zoomin;
            }
            else if ((Button)sender == button4)
            {
                mapActions = GISMapActions.zoomout;
            }
            else if ((Button)sender == button5)
            {
                mapActions = GISMapActions.moveViewDown;
            }
            else if ((Button)sender == button6)
            {
                mapActions = GISMapActions.moveViewUp;
            }
            else if ((Button)sender == button7)
            {
                mapActions = GISMapActions.moveViewRight;
            }
            else if ((Button)sender == button8)
            {
                mapActions = GISMapActions.moveViewLeft;
            }
            _converter.ChangeView(mapActions);
            this.DrawMap();



        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (_layer != null)
                UpdateAndDraw(_layer.Extent, ClientRectangle);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(_layer);
            form2.Show();

        }

        private void button10_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;
            string fileName = dialog.FileName;
            MyFiles.WriteFile(_layer, fileName);
            MessageBox.Show("done");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;
            string fileName = dialog.FileName;
            _layer = MyFiles.ReadFile(fileName);
            MessageBox.Show("Read " + _layer.FeatureCount() + " objects");
            _converter.UpdateConverter(_layer.Extent, ClientRectangle);
            DrawMap();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

            if (_layer == null) return;
            GISVertex vertex = _converter.ToMapVertex(new Point(e.X, e.Y));
            GISSelect gs = new GISSelect();
            if (gs.Select(vertex, _layer.GetAllFeatures(), _layer.ShapeType, _converter) == SelectResult.Ok)
            {
                MessageBox.Show(gs.SelectedFeature.GetAttribute(0).ToString());
            }
        }

    }


}
