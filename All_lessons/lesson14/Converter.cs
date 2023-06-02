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
        GISMapExtent _layerMapExtent;
        GISMapExtent _displayMapExtent;
        //public string MyProptery { get; set; }
        Rectangle _clientWindowRectangle;
        double mapDisplayMinXCoo, mapDisplayMinYCoo, mapDisplayMaxXCoo, mapDisplayMaxYCoo;
        int clientWindowHeight, clientWindowWidth;
        double mapCooW, mapCooH;
        double scaleX, scaleY;
        //I just don't understand the point of CopyExtent
        //I just use the updateConverter Method, it update both two 
        //parameter "extent" and "rectangle"
        /*public void UpdateConverterMemberExtent(GISMapExtent extent)
        {
            _currentMapExtent.CopyExtent(extent);
            UpdateConverter(_currentMapExtent, _clientWindowRectangle);
        }*/
        public GISMapExtent GetLayerExtent() { return _layerMapExtent; }
        public GISMapExtent ScreenRectToExtent(int x1, int x2, int y1, int y2)
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
            _layerMapExtent = extent;
            _clientWindowRectangle = rectangle;
            //mapMinX = _currentMapExtent.minX();
            //mapMinY = _currentMapExtent.minY();
            clientWindowWidth = rectangle.Width;
            clientWindowHeight = rectangle.Height;
            //mapW = _currentMapExtent.width();
            //mapH = _currentMapExtent.height();
            scaleX = _layerMapExtent.width() / clientWindowWidth;
            scaleY = _layerMapExtent.height() / clientWindowHeight;
            scaleX = Math.Max(scaleX, scaleY);
            scaleY = scaleX;
            mapCooW = _clientWindowRectangle.Width * scaleX;
            mapCooH = _clientWindowRectangle.Height * scaleY;
            GISVertex center = _layerMapExtent.GetCenter();
            mapDisplayMinXCoo = center.x - mapCooW / 2;
            mapDisplayMinYCoo = center.y - mapCooH / 2;
            mapDisplayMaxXCoo = center.x + mapCooW / 2;
            mapDisplayMaxYCoo = center.y + mapCooH / 2;
            //_displayMapExtent = new GISMapExtent(mapDisplayMinXCoo, mapDisplayMaxXCoo, mapDisplayMinYCoo, mapDisplayMaxYCoo);
        }
        public GISMapExtent GetDisplayExtent()
        {
            //return new GISMapExtent(mapDisplayMinXCoo, mapDisplayMinXCoo + mapCooW, mapDisplayMinYCoo, mapDisplayMinYCoo + mapCooH);
            return _layerMapExtent;
        }
        public void UpdateDisplayExtent(GISMapExtent extent)
        {
            UpdateConverter(extent, _clientWindowRectangle);
        }


        public Point ToScreenPoint(GISVertex vertex)
        {
            double screenX = (vertex.x - mapDisplayMinXCoo) / scaleX;
            double screenY = clientWindowHeight - (vertex.y - mapDisplayMinYCoo) / scaleY;
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
            double MapX = scaleX * point.X + mapDisplayMinXCoo;
            double MapY = scaleY * (clientWindowHeight - point.Y) + mapDisplayMinYCoo;
            return new GISVertex(MapX, MapY);
        }
        public void ChangeView(GISMapActions mapAction)
        {
            _layerMapExtent.ChangeExtent(mapAction);
            UpdateConverter(_layerMapExtent, _clientWindowRectangle);
        }
    }

}
