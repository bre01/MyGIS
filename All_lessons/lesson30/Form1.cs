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

namespace lesson_3_form
{
    public partial class Form1 : Form
    {
        List<GISFeature> features = new List<GISFeature>();
        GISView view;
        public Form1()
        {
            InitializeComponent();
            view = new GISView(new GISExtent(new GISVertex(0, 0), new GISVertex(100, 100)), ClientRectangle);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
            double x = Convert.ToDouble(textBox1.Text);
            double y = Convert.ToDouble(textBox2.Text);
            GISVertex vertex = new GISVertex(x, y);
            GISPoint point = new GISPoint(vertex);
            //get attribute info
            string attribute = textBox3.Text;
            GISAttribute gisAttribute = new GISAttribute();
            gisAttribute.AddValue(attribute);
            // creat a new feature and add it into the 'feature' List
            GISFeature gisFeature = new GISFeature(point, gisAttribute);
            features.Add(gisFeature);
            Graphics graphics = this.CreateGraphics();
            gisFeature.Draw(graphics, view, true, 0);
            /*point.DrawPoint(graphics);
            point.DrawAttribute(graphics);
            points.Add(point);*/


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            /*GISVertex clickedVertex=new GISVertex((double)e.X,(double)e.Y);
            double minDistance = double.MaxValue;
            int findID = -1;
            for (int i=0;i<points.Count;i++)
            {
                double distance = (points[i]).VertexToPoint(clickedVertex);

                if (distance< minDistance)
                {
                    minDistance=distance;
                    findID = i;
                }
                if(minDistance>5 || findID == -1)
                {
                    MessageBox.Show("no point detected");

                }
                else
                {
                    MessageBox.Show(points[findID].attribute);
                }

               

            }*/
            /*GISVertex vertex = new GISVertex((double)e.X, (double)e.Y);
            double mindistance = Double.MaxValue;
            int findId = -1;
            for (int i = 0; i < features.Count; i++)
            {
                double distance = features[i].spatialPart.centroid.GetDistanceThisVToV(vertex);
                if (distance < mindistance)
                {
                    mindistance = distance;
                    findId = i;
                }
            }
            if (mindistance > 5 || findId == -1)
            {
                MessageBox.Show("not deteced");
            }
            else
                MessageBox.Show(features[findId].GetAttribute(0).ToString());*/
            GISVertex mouseLocation = view.ToMapVertex(new Point(e.X, e.Y));
            double mindistance = Double.MaxValue;
            int findId = -1;
            for (int i = 0; i < features.Count; i++)
            {
                double distance = features[i].spatialPart.centroid.GetDistanceThisVToV(mouseLocation);
                if (distance < mindistance)
                {
                    mindistance = distance;
                    findId = i;
                }
            }
            if (findId == -1)
            {
                MessageBox.Show("not spatial object");
                return;
            }
            Point nearestPoint = view.ToScreenPoint(features[findId].spatialPart.centroid);
            int screenDistance = Math.Abs(nearestPoint.X - e.X) + Math.Abs(nearestPoint.Y - e.Y);
            if (screenDistance > 5)
            {
                MessageBox.Show("please click the object nearly");
                return;
            }
            MessageBox.Show("the object's attribute is: \"" + features[findId].GetAttribute(0) + "\"");



        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            double minX = double.Parse(textBox4.Text);
            double minY = double.Parse(textBox5.Text);
            double maxX = double.Parse(textBox6.Text);
            double maxY = double.Parse(textBox7.Text);
            view.Update(new GISExtent(minX, minY, maxX, maxY), ClientRectangle);
            Graphics graphics = CreateGraphics();
            graphics.FillRectangle(new SolidBrush(Color.Black), ClientRectangle);
            for (int i = 0; i < features.Count; i++)
            {
                features[i].Draw(graphics, view, true, 0);
            }

        }
    }
}
