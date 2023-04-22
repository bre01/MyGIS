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

namespace lesson5
{
    public partial class Form1 : Form
    {
        Layer _layer = null;
        MapAndClientConverter _view = null;
        public Form1()
        {
            InitializeComponent();
            _view = new MapAndClientConverter(new GISMapExtent(new GISVertex(0, 0), new GISVertex(100, 100)), ClientRectangle);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            ShapefileTools shapfileTools = new ShapefileTools();
            _layer = shapfileTools.ReadShapefile(@"C:\Users\bre\Downloads\LANDrop\GisData\chap10\thermal.shp");
            string fileName = null;
            _layer.DrawAttributeOrNot = false;
            MessageBox.Show("Fount" + _layer.FeatureCount() + "Points" + fileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _view.UpdateExtent(_layer.Extent);
            UpdateMap();
        }
        private void UpdateMap()
        {
            Graphics graphics = CreateGraphics();
            graphics.FillRectangle(new SolidBrush(Color.Black), ClientRectangle);
            _layer.Draw(graphics,_view);
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
            _view.ChangeView(mapActions);
            this.UpdateMap();



        }

    }

   
}
