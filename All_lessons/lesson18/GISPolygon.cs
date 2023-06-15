using System;
//using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace My.GIS
{
    public class GISPolygon : GISSpatial
    {
        public List<GISVertex> Vertexes;
        public double Area;
        public GISPolygon(List<GISVertex> vertexes)
        {
            Vertexes = vertexes;
            centroid = GISTools.CalculateCentroid(Vertexes);
            mapExtent = GISTools.CalculateExtent(Vertexes);
            Area = GISTools.CalculateArea(Vertexes);
        }
        public override void Draw(Graphics graphics, MapAndClientConverter view, bool Selected, GISThematic thematic)
        {
            Point[] points = GISTools.ToScreenPoints(Vertexes, view);
            graphics.FillPolygon(new SolidBrush(Selected ? GISConst.SelectedPolygonFillColor : thematic.InsideColor), points);
            graphics.DrawPolygon(new Pen(thematic.OutsideColor, thematic.Size), points);

        }
        public bool Include(GISVertex vertex)
        {
            int count = 0;
            for (int i = 0; i < Vertexes.Count; i++)
            {
                if (Vertexes[i].IsSame(vertex)) return false;
                int next = (i + 1) % Vertexes.Count;
                double minX = Math.Min(Vertexes[i].x, Vertexes[next].x);
                double minY = Math.Min(Vertexes[i].y, Vertexes[next].y);
                double maxX = Math.Max(Vertexes[i].x, Vertexes[next].x);
                double maxY = Math.Max(Vertexes[i].y, Vertexes[next].y);
                if (minX == maxY)
                {
                    if (minY == vertex.y && vertex.x > minX && vertex.x <= maxX) return false;
                    else continue;
                }
                if (vertex.x > maxX || vertex.y > maxY || vertex.y < minY) continue;
                double x0 = Vertexes[i].x + (vertex.y - Vertexes[i].y) * (Vertexes[next].x - Vertexes[i].x) / (Vertexes[next].y - Vertexes[i].y);
                if (x0 < vertex.x) continue;
                if (x0 == vertex.x) return false;
                if (vertex.y == minY) continue;
                count++;

            }
            return count % 2 != 0;
        }
    }
}