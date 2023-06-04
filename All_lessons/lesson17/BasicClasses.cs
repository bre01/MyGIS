using System;
//using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace My.GIS
{
    public abstract class GISSpatial
    {
        public GISVertex centroid;
        public GISMapExtent mapExtent;
        public abstract void Draw(Graphics graphics, MapAndClientConverter view, bool Selected,GISThematic thematic);
    }
    /*
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
        }
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
    */
    public enum GISMapActions
    {
        zoomin, zoomout,
        moveViewDown, moveViewUp, moveViewRight, moveViewLeft
    }
    public enum S
    {
        Point = 1,
        Line = 3,
        Polygon = 5

    }
    public class GISField
    {
        public Type DataType;
        public string Name;
        public GISField(Type dataType, string name)
        {
            DataType = dataType;
            Name = name;
        }
    }
    public enum ALLTYPES
    {
        System_Boolean,
        System_Byte,
        System_Char,
        System_Decimal,
        System_Double,
        System_Int32,
        System_Int64,
        System_SByte,
        System_Int16,
        System_String,
        System_UInt32,
        System_UInt64,
        System_UInt16,
        System_Single,
    }
    public enum SelectResult
    {
        Ok,
        EmptySet,
        TooFar,
        UnknownType
    }
    public static class GISConst
    {
        public static double MinScreenDistance = 5;
        public static int PointSize = 3;
        public static Color PointColor = Color.Blue;
        public static Color LineColor = Color.CadetBlue;
        public static int LineWidth = 2;
        public static Color PolygonBoundaryColor = Color.White;
        public static Color PolygonFillColor = Color.Gray;
        public static int PolygonBoundaryWidth = 2;
        public static Color SelectedPointColor = Color.Red;
        public static Color SelectedLineColor = Color.Blue;
        public static Color SelectedPolygonFillColor = Color.Yellow;
        public static string SHPFILE = "shp";
        public static string MYFILE = "gis";
        public static string MYDOC = "mydoc";
        public static Color ZoomSelectBoxColor = Color.FromArgb(50, 255, 0, 0);
        public static double ZoomInFactor = 0.8;
        public static double ZoomOutFactor = 0.8;
    }
    public enum MOUSECOMMAND
    {
        Unused,
        Select,
        ZoomIn,
        ZoomOut,
        Pan,
        Zoom,
    }
}
