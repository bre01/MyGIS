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
public class GISLine : GISSpatial
    {

        public List<GISVertex> Vertexes;
        public double Length;
        public GISLine(List<GISVertex> vertexes)
        {
            Vertexes = vertexes;
            centroid = GISTools.CalculateCentroid(Vertexes);
            mapExtent = GISTools.CalculateExtent(Vertexes);
            Length = GISTools.CalculateLength(Vertexes);
        }
        public override void Draw(Graphics graphics, MapAndClientConverter view, bool Selected, GISThematic thematic)
        {
            //
            Point[] points = GISTools.ToScreenPoints(Vertexes, view);
            graphics.DrawLines(new Pen(Selected ? GISConst.SelectedLineColor : thematic.InsideColor, thematic.Size), points);

        }
        public GISVertex GetFromNode()
        {
            return Vertexes[0];
        }
        public GISVertex GetToNode()
        {
            return Vertexes[Vertexes.Count - 1];
        }
        public double GetDistanceFromThisToVertex(GISVertex vertex)
        {
            double distance = double.MaxValue;
            for (int i = 0; i < Vertexes.Count - 1; i++)
            {
                distance = Math.Min(GISTools.PointToSegment(Vertexes[i], Vertexes[i + 1], vertex), distance);
            }
            return distance;
        }
    }
}