using System;
using System.Collections;
//using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Deployment.Application;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;

namespace My.GIS
{
public class GISAttribute
    {
        public ArrayList values = new ArrayList();
        public void AddValue(object o)
        {
            values.Add(o);
        }
        public object GetValue(int index)
        {
            return values[index];
        }
        public void Draw(Graphics graphics, MapAndClientConverter view, GISVertex location, int index)
        {
            Point screenPoint = view.ToScreenPoint(location);
            graphics.DrawString(values[index].ToString(), new Font("", 20),
                new SolidBrush(Color.Green), new PointF((int)(screenPoint.X), (int)(screenPoint.Y)));
        }
        public int ValueCount()
        {
            return values.Count;
        }
    }
}