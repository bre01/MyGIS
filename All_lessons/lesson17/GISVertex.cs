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
public class GISVertex
    {
        public double y;
        public double x;
        //constructor
        public GISVertex(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public GISVertex(BinaryReader br)
        {
            x = br.ReadDouble();
            y = br.ReadDouble();
        }
        public GISVertex(GISVertex v)
        {
            CopyVertex(v);
        }


        public double GetDistanceThisVToV(GISVertex gISVertex)
        {
            return Math.Sqrt((x - gISVertex.x) * (x - gISVertex.x) + (y - gISVertex.y) * (y - gISVertex.y));
        }
        public void CopyVertex(GISVertex vertex
            )
        {
            this.x = vertex.x;
            this.y = vertex.y;
            // we can upgrade by add " this.z=vertex.z" and not have to mess the derived method
        }
        public void WriteVertex(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(x);
            binaryWriter.Write(y);
        }
        public bool IsSame(GISVertex vertex)
        {
            return x == vertex.x && y == vertex.y;
        }
    }
}