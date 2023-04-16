using System;
using System.Collections;
//using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

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
    class GISPoint : GISSpatial {
        public GISPoint(GISVertex vertext)
        {
            centroid = vertext;
            extent = new GISExtent(vertext, vertext);
        }
        public override void Draw(Graphics graphics)
        {
            graphics.FillEllipse(new SolidBrush(Color.Red), new Rectangle((int)centroid.x - 3, (int)centroid.y - 3, 6, 6));
        }
        public double GetDistanceThisPointToVertex(GISVertex vertex)
        {
            return centroid.GetDistanceThisVToV(vertex);
        }
    }
    class GISLine : GISSpatial
    {
        List<GISVertex> AllVertexs;
        public override void Draw(Graphics graphics)
        {
            throw new NotImplementedException();
        }
    }
    class GISPolygon : GISSpatial
    {
        List<GISVertex> Allvertexs;
        public override void Draw(Graphics graphics)
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
        public void Draw(Graphics graphics,bool drawAttributeOrNot,int index)
        {
            spatialPart.Draw(graphics);
            if (drawAttributeOrNot)
            {
                attributePart.Draw(graphics, spatialPart.centroid, index);
                attributePart.Draw(graphics, spatialPart.centroid, index);
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
        public void Draw(Graphics graphics,GISVertex location,int index)
        {
            graphics.DrawString(values[index].ToString(),new Font("",20),
                new SolidBrush(Color.Green),new PointF((int)(location.x),(int)(location.y)));
        }
    }
    abstract class GISSpatial
    {
        public GISVertex centroid;
        public GISExtent extent;
        public abstract void Draw(Graphics graphics);
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
    }
}
