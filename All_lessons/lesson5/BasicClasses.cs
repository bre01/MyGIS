using System;
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
        //map coordinates is the real coordinates
        public GISVertex mapBottomLeft;
        public GISVertex mapUpRight;
        public void CopyExtent(GISMapExtent extent)
        {
            mapUpRight.CopyVertex(extent.mapUpRight);
            mapBottomLeft.CopyVertex(extent.mapBottomLeft);
        }
        public GISMapExtent(GISVertex bottomLeft, GISVertex upRight)
        {
            this.mapBottomLeft = bottomLeft;
            this.mapUpRight = upRight;
        }
        public GISMapExtent(double x1, double x2, double y1, double y2)
        {
            //so the order of the parameter doesn't matter
            //we use Math.Max() to determine which one is RightUp or BottomLeft
            mapUpRight = new GISVertex(Math.Max(x1, x2), Math.Max(y1, y2));
            mapBottomLeft = new GISVertex(Math.Min(x1, x2), Math.Min(y1, y2));
        }
        // these all all properties, which I didn'e even notice, I'm so stupid
        public double minX() { return mapBottomLeft.x; }
        public double maxX() { return mapUpRight.x; }

        public double minY() { return mapBottomLeft.y; }
        public double maxY() { return mapUpRight.y; }
        public double width() { return mapUpRight.x - mapBottomLeft.x; }
        public double height() { return mapUpRight.y - mapBottomLeft.y; }
        double zoomFactor = 2;
        double movingFactor = 0.25;
        //read only property to represent  map extent using bottom-left and up-right
        /*public double minX { get { return mapBottomLeft.y; } }
        public double minY { get { return mapBottomLeft.y; } }
        public double maxY { get { return mapUpRight.y; } }
        public double maxX { get { return mapUpRight.x; } }*/

        public void ChangeExtent(GISMapActions action)
        {
            double newMapMinX = mapBottomLeft.x;
            double newMapMinY = mapBottomLeft.y;
            double newMapMaxX = mapUpRight.x;
            double newMapMaxY = mapUpRight.y;
            switch (action)
            {
                //min is bottom left 
                //max is up right
                case GISMapActions.zoomin:
                    newMapMinX = ((minX() + maxX()) - width() / zoomFactor) / 2;
                    newMapMinY = ((minY() + maxX()) - height() / zoomFactor) / 2;
                    newMapMaxX = ((minX() + maxX()) + width() / zoomFactor) / 2;
                    newMapMaxY = ((minY() + maxY()) + height() / zoomFactor) / 2;
                    break;
                case GISMapActions.zoomout:
                    newMapMinX = ((minX() + maxX()) - width() * zoomFactor) / 2;
                    newMapMinY = ((minY() + maxY()) - height() * zoomFactor) / 2;
                    newMapMaxX = ((minX() + maxX()) + height() * zoomFactor) / 2;
                    newMapMaxY = ((minX() + maxX()) + width() * zoomFactor) / 2;
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
        public void UpdateExtent(GISMapExtent extent)
        {
            currentMapExtent.CopyExtent(extent);
            Update(currentMapExtent, ClientWindowRectangle);
        }
        public MapAndClientConverter(GISMapExtent extent, Rectangle clientWindowsRectangle)// current map extent and the client rectangle
        {
            Update(extent, clientWindowsRectangle);
        }
        public void Update(GISMapExtent extent, Rectangle rectangle)
        {
            this.currentMapExtent = extent;
            this.ClientWindowRectangle = rectangle;
            mapMinX = currentMapExtent.minX();
            mapMinY = currentMapExtent.minY();
            clientWindowWidth = rectangle.Width;
            clientWindowHeight = rectangle.Height;
            mapW = currentMapExtent.width();
            mapH = currentMapExtent.height();
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
            currentMapExtent.ChangeExtent(mapAction);
            Update(currentMapExtent, ClientWindowRectangle);
        }
    }
    enum GISMapActions
    {
        zoomin, zoomout,
        moveViewDown, moveViewUp, moveViewRight, moveViewLeft
    }
    public enum ShapeType
    {
        point = 1,
        line = 3,
        polygon = 5

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
                if (shapeType == ShapeType.point)
                {
                    GISPoint point = ReadPoint(RecordContent);
                    GISFeature feature = new GISFeature(point, new GISAttribute());
                    layer.AddFeature(feature);
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

}
