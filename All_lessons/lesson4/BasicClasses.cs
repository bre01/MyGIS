using System;
using System.Collections;
//using System.Collections;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
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
            mapExtent = new GISMapExtent(vertext, vertext);
        }
        public override void Draw(Graphics graphics, MapAndClientConverter view)
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
        public override void Draw(Graphics graphics, MapAndClientConverter view)
        {
            throw new NotImplementedException();
        }
    }
    class GISPolygon : GISSpatial
    {
        List<GISVertex> Allvertexs;
        public override void Draw(Graphics graphics, MapAndClientConverter view)
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
        public void Draw(Graphics graphics, MapAndClientConverter view, bool drawAttributeOrNot, int index)
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
        public void Draw(Graphics graphics, MapAndClientConverter view, GISVertex location, int index)
        {
            Point screenPoint = view.ToScreenPoint(location);
            graphics.DrawString(values[index].ToString(), new Font("", 20),
                new SolidBrush(Color.Green), new PointF((int)(screenPoint.X), (int)(screenPoint.Y)));
        }
    }
    abstract class GISSpatial
    {
        public GISVertex centroid;
        public GISMapExtent mapExtent;
        public abstract void Draw(Graphics graphics, MapAndClientConverter view);
    }
    class GISMapExtent
    {
        public GISVertex mapBottomLeft;
        public GISVertex mapUpRight;
        public GISMapExtent(GISVertex bottomLeft, GISVertex upRight)
        {
            this.mapBottomLeft = bottomLeft;
            this.mapUpRight = upRight;
        }
        public GISMapExtent(double x1, double y1, double x2, double y2)
        {
            mapUpRight = new GISVertex(Math.Max(x1, x2), Math.Max(y1, y2));
            mapBottomLeft = new GISVertex(Math.Min(x1, x2), Math.Min(y1, y2));
        }
        public double GetMapMinX()
        {
            return mapBottomLeft.x;
        }
        public double GetMapMaxX() { return mapUpRight.x; }

        public double GetMapMinY() { return mapBottomLeft.y; }
        public double GetMapMaxY() { return mapUpRight.y; }
        public double GetMapWidth() { return mapUpRight.x - mapBottomLeft.x; }
        public double GetMapHeight() { return mapUpRight.y - mapBottomLeft.y; }
        double zoomFactor = 2;
        public void ChangeExtent(GISMapActions action)
        {
            double newMapMinX = mapBottomLeft.x;
            double newMapMinY = mapBottomLeft.y;
            double newMapMaxX = mapUpRight.x;
            double newMapMaxY = mapUpRight.y;
            double movingFactor = 0.25; 
            switch (action)
            {
                case GISMapActions.zoomin:
                    newMapMinX = ((GetMapMinX() + GetMapMaxX()) - GetMapWidth() / zoomFactor) / 2;
                    newMapMinY = ((GetMapMinY() + GetMapMaxX()) - GetMapHeight() / zoomFactor) / 2;
                    newMapMaxX = ((GetMapMinX() + GetMapMaxX()) + GetMapWidth() / zoomFactor) / 2;
                    newMapMaxY=((GetMapMinY() + GetMapMaxY()) + GetMapHeight() / zoomFactor) / 2;
                    break;
                case GISMapActions.zoomout:
                    break;
                case GISMapActions.moveup:
                    newMapMinY = GetMapMinY() - GetMapHeight() * movingFactor;
                    newMapMaxY = GetMapMaxY() - GetMapHeight() * movingFactor;
                    break;
                case GISMapActions.movedown:
                    newMapMinY += GetMapHeight ()* movingFactor;

                    newMapMaxY += GetMapHeight ()* movingFactor;
                    break;
                case GISMapActions.moveleft:
                    newMapMinX = GetMapMinX() + GetMapWidth() * movingFactor;
                    newMapMaxX = GetMapMaxX() + GetMapWidth() * movingFactor;
                    break;
                case GISMapActions.moveright:
                    newMapMaxX -= GetMapWidth() * movingFactor;
                    newMapMaxX-= GetMapWidth() * movingFactor;
                    break;

            }
            mapUpRight.x = newMapMaxX;
            mapUpRight.y = newMapMaxY;
            mapBottomLeft.x = newMapMinX;
            mapBottomLeft.y = newMapMinY;
        }

    }
    class MapAndClientConverter
    {
        GISMapExtent currentMapExtent;
        public string MyProptery { get; set; }
        Rectangle ClientWindowRectangle;
        double mapMinX, mapMinY;
        int clientWindowHeight, clientWindowWidth;
        double mapW, mapH;
        double scaleX, scaleY;

        public MapAndClientConverter(GISMapExtent extent, Rectangle clientWindowsRectangle)// current map extent and the client rectangle
        {
            Update(extent, clientWindowsRectangle);
        }
        public void Update(GISMapExtent extent, Rectangle rectangle)
        {
            this.currentMapExtent = extent;
            this.ClientWindowRectangle = rectangle;
            mapMinX = currentMapExtent.GetMapMinX();
            mapMinY = currentMapExtent.GetMapMinY();
            clientWindowWidth = rectangle.Width;
            clientWindowHeight = rectangle.Height;
            mapW = currentMapExtent.GetMapWidth();
            mapH = currentMapExtent.GetMapHeight();
            scaleX = mapW / clientWindowWidth;
            scaleY = mapH / clientWindowHeight;

        }
        public Point ToScreenPoint(GISVertex vertex)
        {
            double screenX = (vertex.x - mapMinX) / scaleX;
            double screenY = clientWindowHeight - (vertex.x - mapMinY) / scaleY;
            return new Point((int)screenX, (int)screenY);
        }
        public GISVertex ToMapVertex(Point point)
        {
            double MapX = scaleX * point.X + mapMinX;
            double MapY = scaleY * (clientWindowHeight - point.Y) + mapMinY;
            return new GISVertex(MapX, MapY);
        }
        public void ChangeView(GISMapActions mapAction)
        {
            currentMapExtent.ChangeExtent(mapAction);
            Update(currentMapExtent, ClientWindowRectangle);
        }
    }
    enum GISMapActions
    {
        zoomin, zoomout,
        moveup, movedown, moveleft, moveright
    }

}
