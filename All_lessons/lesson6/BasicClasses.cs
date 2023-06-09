﻿using System;
using System.Collections;
//using System.Collections;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
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
        public void CopyVertex(GISVertex vertex
            )
        {
            this.x = vertex.x;
            this.y = vertex.y;
            // we can upgrade by add " this.z=vertex.z" and not have to mess the derived method
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

        private List<GISVertex> _vertexes;
        public double Length;
        public GISLine(List<GISVertex> vertexes)
        {
            _vertexes = vertexes;
            centroid = CalTool.CalculateCentroid(_vertexes);
            mapExtent = CalTool.CalculateExtent(_vertexes);
            Length = CalTool.CalculateLength(_vertexes);
        }
        public override void Draw(Graphics graphics, MapAndClientConverter view)
        {
            //
            Point[] points = CalTool.ToScreenPoints(_vertexes, view);
            graphics.DrawLines(new Pen(Color.Red, 2), points);

        }
        public GISVertex GetFromNode()
        {
            return _vertexes[0];
        }
        public GISVertex GetToNode()
        {
            return _vertexes[_vertexes.Count - 1];
        }
    }
    class GISPolygon : GISSpatial
    {
        private List<GISVertex> _vertexes;
        public double Area;
        public GISPolygon(List<GISVertex> vertexes)
        {
            _vertexes = vertexes;
            centroid = CalTool.CalculateCentroid(_vertexes);
            mapExtent = CalTool.CalculateExtent(_vertexes);
            Area = CalTool.CalculateArea(_vertexes);
        }
        public override void Draw(Graphics graphics, MapAndClientConverter view)
        {
            Point[] points = CalTool.ToScreenPoints(_vertexes, view);
            graphics.FillPolygon(new SolidBrush(Color.Yellow), points);
            graphics.DrawPolygon(new Pen(Color.White, 2), points);

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
        //map coordinates is the real coordinates
        public GISVertex MapBottomLeft;
        public GISVertex MapUpRight;
        public void CopyExtent(GISMapExtent extent)
        {
            MapUpRight.CopyVertex(extent.MapUpRight);
            MapBottomLeft.CopyVertex(extent.MapBottomLeft);
        }
        public GISMapExtent(GISVertex bottomLeft, GISVertex upRight)
        {
            MapBottomLeft = bottomLeft;
            MapUpRight = upRight;
        }
        public GISMapExtent(double x1, double x2, double y1, double y2)
        {
            //so the order of the parameter doesn't matter
            //we use Math.Max() to determine which one is RightUp or BottomLeft
            MapUpRight = new GISVertex(Math.Max(x1, x2), Math.Max(y1, y2));
            MapBottomLeft = new GISVertex(Math.Min(x1, x2), Math.Min(y1, y2));
        }
        // these all all properties, which I didn'e even notice, I'm so stupid
        public double minX() { return MapBottomLeft.x; }
        public double maxX() { return MapUpRight.x; }

        public double minY() { return MapBottomLeft.y; }
        public double maxY() { return MapUpRight.y; }
        public double width() { return MapUpRight.x - MapBottomLeft.x; }
        public double height() { return MapUpRight.y - MapBottomLeft.y; }
        double zoomFactor = 2;
        double movingFactor = 0.25;
        //read only property to represent  map extent using bottom-left and up-right
        /*public double minX { get { return mapBottomLeft.y; } }
        public double minY { get { return mapBottomLeft.y; } }
        public double maxY { get { return mapUpRight.y; } }
        public double maxX { get { return mapUpRight.x; } }*/

        public void ChangeExtent(GISMapActions action)
        {
            double newMapMinX = MapBottomLeft.x;
            double newMapMinY = MapBottomLeft.y;
            double newMapMaxX = MapUpRight.x;
            double newMapMaxY = MapUpRight.y;
            switch (action)
            {
                //min is bottom left 
                //max is up right
                case GISMapActions.zoomin:
                    newMapMinX = ((minX() + maxX()) - width() / zoomFactor) / 2;
                    newMapMinY = ((minY() + maxY()) - height() / zoomFactor) / 2;
                    newMapMaxX = ((minX() + maxX()) + width() / zoomFactor) / 2;
                    newMapMaxY = ((minY() + maxY()) + height() / zoomFactor) / 2;
                    break;
                case GISMapActions.zoomout:
                    newMapMinX = ((minX() + maxX()) - width() * zoomFactor) / 2;
                    newMapMinY = ((minY() + maxY()) - height() * zoomFactor) / 2;
                    newMapMaxX = ((minX() + maxX()) + width() * zoomFactor) / 2;
                    newMapMaxY = ((minY() + maxY()) + height() * zoomFactor) / 2;
                    break;
                case GISMapActions.moveViewDown:
                    newMapMinY = minY() - height() * movingFactor;
                    newMapMaxY = maxY() - height() * movingFactor;
                    break;
                case GISMapActions.moveViewUp:
                    newMapMinY = minY() + height() * movingFactor;

                    newMapMaxY = maxY() + height() * movingFactor;
                    break;
                case GISMapActions.moveViewRight:
                    newMapMinX = minX() + width() * movingFactor;
                    newMapMaxX = maxX() + width() * movingFactor;
                    break;
                case GISMapActions.moveViewLeft:
                    newMapMaxX = maxX() - width() * movingFactor;
                    newMapMinX = minX() - width() * movingFactor;
                    break;

            }
            MapUpRight.x = newMapMaxX;
            MapUpRight.y = newMapMaxY;
            MapBottomLeft.x = newMapMinX;
            MapBottomLeft.y = newMapMinY;
        }

    }
    class MapAndClientConverter
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

        public MapAndClientConverter(GISMapExtent extent, Rectangle clientWindowRectangle)// current map extent and the client rectangle
        {
            UpdateConverter(extent, clientWindowRectangle);
        }

        public void UpdateConverter(GISMapExtent extent, Rectangle rectangle)
        {
            _currentMapExtent = extent;
            _clientWindowRectangle = rectangle;
            mapMinX = _currentMapExtent.minX();
            mapMinY = _currentMapExtent.minY();
            clientWindowWidth = rectangle.Width;
            clientWindowHeight = rectangle.Height;
            mapW = _currentMapExtent.width();
            mapH = _currentMapExtent.height();
            scaleX = mapW / clientWindowWidth;
            scaleY = mapH / clientWindowHeight;

        }
        public Point ToScreenPoint(GISVertex vertex)
        {
            double screenX = (vertex.x - mapMinX) / scaleX;
            double screenY = clientWindowHeight - (vertex.y - mapMinY) / scaleY;
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
            _currentMapExtent.ChangeExtent(mapAction);
            UpdateConverter(_currentMapExtent, _clientWindowRectangle);
        }
    }
    enum GISMapActions
    {
        zoomin, zoomout,
        moveViewDown, moveViewUp, moveViewRight, moveViewLeft
    }
    public enum ShapeType
    {
        Point = 1,
        Line = 3,
        Polygon = 5

    }

    class ShapefileTools
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct ShapefileHeader
        {
            public int Unused1, Unused2, Unused3, Unused4;
            public int Unused5, Unused6, Unused7, Unused8;
            public int ShapeType;
            public double Xmin;
            public double Ymin;
            public double Xmax;
            public double Ymax;
            public double Unused9, Unused10, Unused11, Unused12;

        }
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct RecordHeader
        {
            public int RecordNumber;
            public int RecordLenght;
            public int ShapeType;//it's a repeat the type in header file 
            // because only one type in a shapefile
        }
        RecordHeader ReadRecordHeader(BinaryReader br)
        {
            byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(RecordHeader))); //i have a buff
            GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned); //move buff to handle
            RecordHeader header = (RecordHeader)Marshal.PtrToStructure
                (handle.AddrOfPinnedObject(), typeof(RecordHeader));//move handle to header
            handle.Free();
            return header;

        }
        int FromBigToLittle(int bigValue)
        {
            byte[] bigBytes = new byte[4];
            GCHandle handle = GCHandle.Alloc(bigBytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(bigValue, handle.AddrOfPinnedObject(), false);
            handle.Free();
            byte b2 = bigBytes[2];
            byte b3 = bigBytes[3];
            bigBytes[3] = bigBytes[0];
            bigBytes[2] = bigBytes[1];
            bigBytes[1] = b2;
            bigBytes[0] = b3;
            return BitConverter.ToInt32(bigBytes, 0);
        }

        ShapefileHeader ReadShapfileHeader(BinaryReader br)
        {
            byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(ShapefileHeader))); //i have a buff
            GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned); //move buff to handle
            ShapefileHeader header = (ShapefileHeader)Marshal.PtrToStructure
                (handle.AddrOfPinnedObject(), typeof(ShapefileHeader));//move handle to header
            handle.Free();
            return header;
            /*  my understanding
             * read a some bytes into buff, and pin the buff to handle 

            and move the bytes pinned in handle to header
            free the handle
            then get(return) the header*/
            /* understanding from the book
             * so the handle actually get the address of the "buff" array 
             * and we directly make the address of ShapefileHeader the same as the handle
             * then release the handle, so the memory is now controled by .net, and we don't need to 
             * manage the memory, because we can use GC.*/
        }
        //after get the header, we can now read the members  (after the header file)
        //we create a method whichs utilzes the "ReadShapefileHeader" method,
        //and then read the member
        public Layer ReadShapefile(string shapefileName)
        {
            FileStream fsr = new FileStream(shapefileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fsr); // BinaryReader Constructors has three overloads
            //one that takes only one paramter using UTF-8 encoding by default
            ShapefileHeader header = ReadShapfileHeader(br);
            //int shapeType = header.ShapeType;
            ShapeType shapeType = (ShapeType)Enum.Parse(typeof(ShapeType),
                header.ShapeType.ToString());
            GISMapExtent extent = new GISMapExtent(header.Xmax, header.Xmin, header.Ymax, header.Ymin);
            Layer layer = new Layer(shapefileName, shapeType, extent);
            while (br.PeekChar() != -1)
            {
                RecordHeader rh = ReadRecordHeader(br);
                int RecordLength = FromBigToLittle(rh.RecordLenght) * 2 - 4;//some modfication 
                //to better reflect the real length
                byte[] RecordContent = br.ReadBytes(RecordLength);
                if (shapeType == ShapeType.Point)
                {
                    GISPoint point = ReadPoint(RecordContent);
                    GISFeature feature = new GISFeature(point, new GISAttribute());
                    layer.AddFeature(feature);
                }
                if (shapeType == ShapeType.Line)
                {
                    List<GISLine> lines = ReadLines(RecordContent);
                    for (int i = 0; i < lines.Count; i++)
                    {
                        GISFeature feature = new GISFeature(lines[i], new GISAttribute());
                        layer.AddFeature(feature);
                    }
                }
                if (shapeType == ShapeType.Polygon)
                {
                    List<GISPolygon> polygons = ReadPolygons(RecordContent);
                    for (int i = 0; i < polygons.Count; i++)
                    {
                        GISFeature feature = new GISFeature(polygons[i], new GISAttribute());
                        layer.AddFeature(feature);
                    }
                }

            }
            br.Close();
            fsr.Close();
            return layer;
        }

        public GISPoint ReadPoint(byte[] recordContent)
        {
            double x = BitConverter.ToDouble(recordContent, 0);
            double y = BitConverter.ToDouble(recordContent, 8);
            return new GISPoint(new GISVertex(x, y));
        }
        public List<GISLine> ReadLines(byte[] RecordContent)
        {
            int shapeCount = BitConverter.ToInt32(RecordContent, 32);// how many shapes
            int vertexCount = BitConverter.ToInt32(RecordContent, 36);//how many vertex
            //every vertex has a "x" and "y", a "x" or "y" takes up 4 byte
            int vertexCoordinateBeginByte = 40 + shapeCount * 4;
            int[] shapeBeginLocation = new int[shapeCount + 1];
            for (int i = 0; i < shapeCount; i++)
            {
                shapeBeginLocation[i] = BitConverter.ToInt32(RecordContent, 40 + i * 4);

            }
            shapeBeginLocation[shapeCount] = vertexCount;// last vertex location
            List<GISLine> lines = new List<GISLine>();
            for (int i = 0; i < shapeCount; i++)
            {
                List<GISVertex> vertexes = new List<GISVertex>();
                for (int j = shapeBeginLocation[i]; j < shapeBeginLocation[i + 1]; j++)
                {
                    double x = BitConverter.ToDouble(RecordContent, vertexCoordinateBeginByte + 16 * j);
                    double y = BitConverter.ToDouble(RecordContent, vertexCoordinateBeginByte + 16 * j + 8);
                    vertexes.Add(new GISVertex(x, y));
                }
                lines.Add(new GISLine(vertexes));
            }
            return lines;
        }
        public List<GISPolygon> ReadPolygons(byte[] RecordContent)
        {
            int shapeCount = BitConverter.ToInt32(RecordContent, 32);// how many shapes
            int vertexCount = BitConverter.ToInt32(RecordContent, 36);//how many vertex
            //every vertex has a "x" and "y", a "x" or "y" takes up 4 byte
            int vertexCoordinateBeginByte = 40 + shapeCount * 4;
            int[] shapeBeginLocation = new int[shapeCount + 1];
            for (int i = 0; i < shapeCount; i++)
            {
                shapeBeginLocation[i] = BitConverter.ToInt32(RecordContent, 40 + i * 4);

            }
            shapeBeginLocation[shapeCount] = vertexCount;// last vertex location
            List<GISPolygon> polygons = new List<GISPolygon>();
            for (int i = 0; i < shapeCount; i++)
            {
                List<GISVertex> vertexes = new List<GISVertex>();
                for (int j = shapeBeginLocation[i]; j < shapeBeginLocation[i + 1]; j++)
                {
                    double x = BitConverter.ToDouble(RecordContent, vertexCoordinateBeginByte + 16 * j);
                    double y = BitConverter.ToDouble(RecordContent, vertexCoordinateBeginByte + 16 * j + 8);
                    vertexes.Add(new GISVertex(x, y));
                }
                polygons.Add(new GISPolygon(vertexes));
            }
            return polygons;
        }


    }

    internal class Layer
    {

        /// <naming_convention>
        /// if it's PascalCase, it's public member (of the class)
        /// if it's _camelCase, it's private or internal member (of the class)
        /// if it's camelCase,  it's local variable or parameter (of the method)
        /// </naming_convention>
        public string name;
        public GISMapExtent Extent;
        public bool DrawAttributeOrNot;
        public int LabelIndex;
        public ShapeType ShapeType;
        private List<GISFeature> _features = new List<GISFeature>();
        public Layer(string _name, ShapeType _shapetype, GISMapExtent _extent)
        {
            name = _name;
            ShapeType = _shapetype;
            Extent = _extent;
        }
        public void Draw(Graphics graphics, MapAndClientConverter view)
        {
            for (int i = 0; i < _features.Count; i++)
            {
                _features[i].Draw(graphics, view, this.DrawAttributeOrNot, this.LabelIndex);
            }
        }
        public void AddFeature(GISFeature feature)
        {
            _features.Add(feature);
        }
        public int FeatureCount()
        {
            return _features.Count;
        }
    }
    static class CalTool
    {
        public static GISVertex CalculateCentroid(List<GISVertex> vertexes)
        {
            if (vertexes.Count == 0) return null;
            double x = 0, y = 0;
            for (int i = 0; i < vertexes.Count; i++)
            {
                x += vertexes[i].x;
                y += vertexes[i].y;
            }
            return new GISVertex(x / vertexes.Count, y / vertexes.Count);
        }
        public static GISMapExtent CalculateExtent(List<GISVertex> vertexes)
        {
            if (vertexes.Count == 0) return null;
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i].x < minX) minX = vertexes[i].x;
                if (vertexes[i].y < minY) minY = vertexes[i].y;
                if (vertexes[i].x > maxX) maxX = vertexes[i].x;
                if (vertexes[i].y > maxY) maxY = vertexes[i].y;
            }
            return new GISMapExtent(minX, maxX, minY, maxY);
        }

        public static double CalculateLength(List<GISVertex> vertexes)
        {
            double length = 0;
            for (int i = 0; i < vertexes.Count - 1; i++)
            {
                length += vertexes[i].GetDistanceThisVToV(vertexes[i + 1]);
            }
            return length;
        }
        public static double CalculateArea(List<GISVertex> vertexes)
        {
            double area = 0;
            for (int i = 0; i < vertexes.Count - 1; i++)
            {
                area += VectorProduct(vertexes[i], vertexes[i + 1]);
            }
            return area;
        }
        public static double VectorProduct(GISVertex v1, GISVertex v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }
        public static Point[] ToScreenPoints(List<GISVertex> vertexes, MapAndClientConverter view)
        {
            Point[] points = new Point[vertexes.Count];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = view.ToScreenPoint(vertexes[i]);
            }
            return points;
        }
    }

}
