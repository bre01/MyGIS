using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyGis;

namespace lesson1
{
    public partial class Form1 : Form
    {
        List<GISPoint> points = new List<GISPoint>();
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //get spatial info
            double x=Convert.ToDouble(textBox1.Text);
            double y=Convert.ToDouble(textBox2.Text);
            string attribute=textBox3.Text;
            GISVertex vertex = new GISVertex(x, y);
            GISPoint point = new GISPoint(vertex,"string");
            Graphics graphics = this.CreateGraphics();
            point.DrawPoint(graphics);
            point.DrawAttribute(graphics);
            points.Add(point);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            GISVertex clickedVertex=new GISVertex((double)e.X,(double)e.Y);
            double minDistance = double.MaxValue;
            int findID = -1;
            for (int i = 0; i < points.Count; i++)
            {
                double distance = (points[i]).VertexToPoint(clickedVertex);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    findID = i;
                }
            }
                if(minDistance>5 || findID == -1)
                {
                    MessageBox.Show("no point detected");

                }
                else
                {
                    MessageBox.Show(points[findID].attribute);
                }

               

        }

    }
}
