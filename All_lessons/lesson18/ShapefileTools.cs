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
public class ShapefileTools
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
            S shapeType = (S)Enum.Parse(typeof(S),
                header.ShapeType.ToString());
            GISMapExtent extent = new GISMapExtent(header.Xmax, header.Xmin, header.Ymax, header.Ymin);

            //Layer layer = new Layer(shapefileName, shapeType, extent);
            string dbfFilename = shapefileName.Replace(".shp", ".dbf");
            DataTable table = GISTools.ReadDBF(dbfFilename);
            Layer layer = new Layer(shapefileName, shapeType, extent, GISTools.ReadFields(table));
            int rowIndex = 0;
            while (br.PeekChar() != -1)
            {
                RecordHeader rh = ReadRecordHeader(br);
                int RecordLength = FromBigToLittle(rh.RecordLenght) * 2 - 4;//some modfication 
                //to better reflect the real length
                byte[] RecordContent = br.ReadBytes(RecordLength);
                if (shapeType == S.Point)
                {
                    GISPoint point = ReadPoint(RecordContent);
                    GISFeature feature = new GISFeature(point, GISTools.ReadAttribute(table, rowIndex));
                    layer.AddFeature(feature);
                }
                if (shapeType == S.Line)
                {
                    List<GISLine> lines = ReadLines(RecordContent);
                    for (int i = 0; i < lines.Count; i++)
                    {
                        GISFeature feature = new GISFeature(lines[i], GISTools.ReadAttribute(table, rowIndex));
                        layer.AddFeature(feature);
                    }
                }
                if (shapeType == S.Polygon)
                {
                    List<GISPolygon> polygons = ReadPolygons(RecordContent);
                    for (int i = 0; i < polygons.Count; i++)
                    {
                        GISFeature feature = new GISFeature(polygons[i], GISTools.ReadAttribute(table, rowIndex));
                        layer.AddFeature(feature);
                    }
                }


                rowIndex++;

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
}