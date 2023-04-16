using System;
using System.Collections;
//using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace MyGis
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

        public double GetDistanceThisVToV(GISVertex gISVertex)
        {
            return Math.Sqrt((x - gISVertex.x) * (x - gISVertex.x) + (y - gISVertex.y) * (y - gISVertex.y));
        }
    }
    /*
     class GISPoint
    {
        public GISVertex location;
        public string attribute;
        //constructor
        public GISPoint(GISVertex gISVertex, string pointAttribute)
        {
            location = gISVertex;
            attribute = pointAttribute;
        }
        public void DrawPoint(Graphics graphics)
        {
            graphics.FillEllipse(new SolidBrush(Color.Red),
                new Rectangle((int)(location.x)-3,(int)(location.y)-3,6,6));
        }
        public void DrawAttribute(Graphics graphics)
        {
            graphics.DrawString(attribute, new Font("LXGW", 20), new SolidBrush(Color.Green),
                new PointF((int)location.x, (int)(location.y)));
        }
        public double VertexToPoint(GISVertex anotherVertex)
        {
            return location.DistanceToVertex(anotherVertex);
        }

    }
    */
    class GISPoint : GISSpatial
    {
        public GISPoint(GISVertex vertext)
        {
            centroid = vertext;
            extent = new GISExtent(vertext, vertext);
        }
        public override void Draw(Graphics graphics, GISView view)
        {
            Point screenPoint = view.ToScreenPoint(centroid);

            graphics.FillEllipse(new SolidBrush(Color.Red), new Rectangle((int)screenPoint.X - 3, (int)screenPoint.Y - 3, 6, 6));
        }
        public double GetDistanceThisPointToVertex(GISVertex vertex)
        {
            return centroid.GetDistanceThisVToV(vertex);
        }
    }
    class GISLine : GISSpatial
    {
        List<GISVertex> AllVertexs;
        public override void Draw(Graphics graphics, GISView view)
        {
            throw new NotImplementedException();
        }
    }
    class GISPolygon : GISSpatial
    {
        List<GISVertex> Allvertexs;
        public override void Draw(Graphics graphics, GISView view)
        {
            throw new NotImplementedException();
        }
    }
    /*class GISLine
    {
        List<GISVertex> AllVertexs;
    }
    class GISPolygon
    {
        List<GISVertex> AllVertexs;
    }*/
    class GISFeature
    {
        public GISSpatial spatialPart;
        public GISAttribute attributePart;
        public GISFeature(GISSpatial spatial, GISAttribute attribute)
        {
            spatialPart = spatial;
            attributePart = attribute;
        }
        public void Draw(Graphics graphics, GISView view, bool drawAttributeOrNot, int index)
        {
            spatialPart.Draw(graphics, view);
            if (drawAttributeOrNot)
            {
                attributePart.Draw(graphics, view, spatialPart.centroid, index);
            }
        }
        public Object GetAttribute(int index)
        {
            return attributePart.GetValue(index);
        }


    }
    class GISAttribute
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
        public void Draw(Graphics graphics, GISView view, GISVertex location, int index)
        {
            Point screenPoint = view.ToScreenPoint(location);
            graphics.DrawString(values[index].ToString(), new Font("", 20),
                new SolidBrush(Color.Green), new PointF((int)(screenPoint.X), (int)(screenPoint.Y)));
        }
    }
    abstract class GISSpatial
    {
        public GISVertex centroid;
        public GISExtent extent;
        public abstract void Draw(Graphics graphics, GISView view);
    }
    class GISExtent
    {
        public GISVertex bottomLeft;
        public GISVertex upRight;
        public GISExtent(GISVertex bottomLeft, GISVertex upRight)
        {
            this.bottomLeft = bottomLeft;
            this.upRight = upRight;
        }
        public GISExtent(double x1, double y1, double x2, double y2)
        {
            upRight = new GISVertex(Math.Max(x1, x2), Math.Max(y1, y2));
            bottomLeft = new GISVertex(Math.Min(x1, x2), Math.Min(y1, y2));
        }
        public double GetMinX()
        {
            return bottomLeft.x;
        }
        public double GetMaxX() { return upRight.x; }

        public double GetMinY() { return bottomLeft.y; }
        public double GetMaxY() { return upRight.y; }
        public double GetWidth() { return upRight.x - bottomLeft.x; }
        public double GetHeight() { return upRight.y - bottomLeft.y; }

    }
    class GISView
    {
        GISExtent currentMapExtent;
        Rectangle mapWindowSize;
        double mapMinX, mapMinY;
        int winH, winW;
        double mapW, mapH;
        double scaleX, scaleY;

        public GISView(GISExtent extent, Rectangle rectangle)
        {
            Update(extent, rectangle);
        }
        public void Update(GISExtent extent, Rectangle rectangle)
        {
            this.currentMapExtent = extent;
            this.mapWindowSize = rectangle;
            mapMinX = currentMapExtent.GetMinX();
            mapMinY = currentMapExtent.GetMinY();
            winW = rectangle.Width;
            winH = rectangle.Height;
            mapW = currentMapExtent.GetWidth();
            mapH = currentMapExtent.GetHeight();
            scaleX = mapW / winW;
            scaleY = mapH / winH;

        }
        public Point ToScreenPoint(GISVertex vertex)
        {
            double screenX = (vertex.x - mapMinX) / scaleX;
            double screenY = winH - (vertex.x - mapMinY) / scaleY;
            return new Point((int)screenX, (int)screenY);
        }
        public GISVertex ToMapVertex(Point point)
        {
            double MapX = scaleX * point.X + mapMinX;
            double MapY = scaleY * (winH - point.Y) + mapMinY;
            return new GISVertex(MapX, MapY);
        }
    }
}
