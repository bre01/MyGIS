using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.GIS
{
    public class MapAndClientConverter
    {
        GISMapExtent _currentMapExtent;
        //public string MyProptery { get; set; }
        Rectangle _clientWindowRectangle;
        double mapMinX, mapMinY;
        int clientWindowHeight, clientWindowWidth;
        double mapW, mapH;
        double scaleX, scaleY;
        //I just don't understand the point of CopyExtent
        //I just use the updateConverter Method, it update both two 
        //parameter "extent" and "rectangle"
        /*public void UpdateConverterMemberExtent(GISMapExtent extent)
        {
            _currentMapExtent.CopyExtent(extent);
            UpdateConverter(_currentMapExtent, _clientWindowRectangle);
        }*/
        public GISMapExtent RectToExtent(int x1, int x2, int y1, int y2)
        {
            GISVertex v1 = ToMapVertex(new Point(x1, y1));
            GISVertex v2 = ToMapVertex(new Point(x2, y2));
            return new GISMapExtent(v1.x, v2.x, v1.y, v2.y);
        }

        public MapAndClientConverter(GISMapExtent extent, Rectangle clientWindowRectangle)// current map extent and the client rectangle
        {
            UpdateConverter(extent, clientWindowRectangle);
        }

        public void UpdateConverter(GISMapExtent extent, Rectangle rectangle)
        {
            _currentMapExtent = extent;
            _clientWindowRectangle = rectangle;
            //mapMinX = _currentMapExtent.minX();
            //mapMinY = _currentMapExtent.minY();
            clientWindowWidth = rectangle.Width;
            clientWindowHeight = rectangle.Height;
            //mapW = _currentMapExtent.width();
            //mapH = _currentMapExtent.height();
            scaleX = _currentMapExtent.width() / clientWindowWidth;
            scaleY = _currentMapExtent.height() / clientWindowHeight;
            scaleX = Math.Max(scaleX, scaleY);
            scaleY = scaleX;
            mapW = _clientWindowRectangle.Width * scaleX;
            mapH = _clientWindowRectangle.Height * scaleY;
            GISVertex center = _currentMapExtent.GetCenter();
            mapMinX = center.x - mapW / 2;
            mapMinY = center.y - mapH / 2;
        }
        public GISMapExtent GetDisplayExtent()
        {
            return new GISMapExtent(mapMinX, mapMinX + mapW, mapMinY, mapMinY + mapH);
        }
        public void UpdateDisplayExtent(GISMapExtent extent)
        {
            UpdateConverter(extent, _clientWindowRectangle);
        }


        public Point ToScreenPoint(GISVertex vertex)
        {
            double screenX = (vertex.x - mapMinX) / scaleX;
            double screenY = clientWindowHeight - (vertex.y - mapMinY) / scaleY;
            return new Point((int)screenX, (int)screenY);
        }
        public double ToScreenDistance(GISVertex v1, GISVertex v2)
        {
            Point p1 = ToScreenPoint(v1);
            Point p2 = ToScreenPoint(v2);
            return Math.Sqrt((double)((p1.X - p2.X) * (p1.X - p2.X)) + ((p1.Y - p2.Y) * (p1.Y - p2.Y)));
        }
        public double ToScreenDistance(double distance)
        {
            return ToScreenDistance(new GISVertex(0, 0), new GISVertex(0, distance));
        }
        public GISVertex ToMapVertex(Point point)
        {
            double MapX = scaleX * point.X + mapMinX;
            double MapY = scaleY * (clientWindowHeight - point.Y) + mapMinY;
            return new GISVertex(MapX, MapY);
        }
        public void ChangeView(GISMapActions mapAction)
        {
            _currentMapExtent.ChangeExtent(mapAction);
            UpdateConverter(_currentMapExtent, _clientWindowRectangle);
        }
    }

}
