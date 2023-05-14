using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace My.GIS
{
    public class MyFiles

    {

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct MyFileHeader
        {
            public double MinX, MinY, MaxX, MaxY;
            public int FeatureCount, ShapeType, FieldCount;
        }
        static void WriteFileHeader(Layer layer, BinaryWriter binaryWriter)
        {
            MyFileHeader myFileHeader = new MyFileHeader();
            myFileHeader.MinX = layer.Extent.minX();
            myFileHeader.MinY = layer.Extent.minY();
            myFileHeader.MaxX = layer.Extent.maxX();
            myFileHeader.MaxY = layer.Extent.maxY();
            myFileHeader.FeatureCount = layer.FeatureCount();
            myFileHeader.ShapeType = (int)layer.ShapeType;
            myFileHeader.FieldCount = layer.Fields.Count();
            binaryWriter.Write(CalTool.ToBytes(myFileHeader));
        }
        public static void WriteFile(Layer layer, string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            WriteFileHeader(layer, binaryWriter);
            CalTool.WriteString(layer.Name, binaryWriter);
            WriteFields(layer.Fields, binaryWriter);
            WriteFeatures(layer, binaryWriter);
            binaryWriter.Close();
            fileStream.Close();
        }
        static void WriteFields(List<GISField> fields, BinaryWriter binaryWriter)
        {
            for (int fieldIndex = 0; fieldIndex < fields.Count(); fieldIndex++)
            {
                GISField field = fields[fieldIndex];
                binaryWriter.Write(CalTool.TypeToInt(field.DataType));
                CalTool.WriteString(field.Name, binaryWriter);
            }
        }
        static void WriteMultipleVertexes(List<GISVertex> vertexes, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(vertexes.Count());
            for (int vertexIndex = 0; vertexIndex < vertexes.Count(); vertexIndex++)
            {
                vertexes[vertexIndex].WriteVertex(binaryWriter);
            }
        }
        static void WriteAttributes(GISAttribute attribute, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < attribute.ValueCount(); i++)
            {
                Type type = attribute.GetValue(i).GetType();
                if (type.ToString() == "System.Boolean")
                    binaryWriter.Write((bool)attribute.GetValue(i));
                else if (type.ToString() == "System.Byte")
                    binaryWriter.Write((byte)attribute.GetValue(i));
                else if (type.ToString() == "System.Char")
                    binaryWriter.Write((char)attribute.GetValue(i));
                else if (type.ToString() == "System.Deciaml")
                    binaryWriter.Write((decimal)attribute.GetValue(i));
                else if (type.ToString() == "System.Double")
                    binaryWriter.Write((double)attribute.GetValue(i));
                else if (type.ToString() == "System.Single")
                    binaryWriter.Write((float)attribute.GetValue(i));
                else if (type.ToString() == "System.Int32")
                    binaryWriter.Write((int)attribute.GetValue(i));
                else if (type.ToString() == "System.Int64")
                    binaryWriter.Write((long)attribute.GetValue(i));
                else if (type.ToString() == "System.UInt16")
                    binaryWriter.Write((ushort)attribute.GetValue(i));
                else if (type.ToString() == "System.UInt32")
                    binaryWriter.Write((uint)attribute.GetValue(i));
                else if (type.ToString() == "System.UInt64")
                    binaryWriter.Write((ulong)attribute.GetValue(i));
                else if (type.ToString() == "System.SByte")
                    binaryWriter.Write((sbyte)attribute.GetValue(i));
                else if (type.ToString() == "System.Int16")
                    binaryWriter.Write((short)attribute.GetValue(i));
                else if (type.ToString() == "System.String")
                {
                  
                    //binaryWriter.Write((string)attribute.GetValue(i));
                    CalTool.WriteString((string)attribute.GetValue(i), binaryWriter);
                }

            }
        }
        static void WriteFeatures(Layer layer, BinaryWriter binaryWriter)
        {
            for (int featureIndex = 0; featureIndex < layer.FeatureCount(); featureIndex++)
            {
                GISFeature feature = layer.GetFeature(featureIndex);
                if (layer.ShapeType == S.Point)
                {
                    ((GISPoint)feature.spatialPart).centroid.WriteVertex(binaryWriter);
                }
                else if (layer.ShapeType == S.Line)
                {
                    GISLine line = (GISLine)(feature.spatialPart);
                    WriteMultipleVertexes(line.Vertexes, binaryWriter);

                }
                else if (layer.ShapeType == S.Polygon)
                {
                    GISPolygon polygon = (GISPolygon)(feature.spatialPart);
                    WriteMultipleVertexes(polygon.Vertexes, binaryWriter);
                }
                WriteAttributes(feature.attributePart, binaryWriter);
            }
        }


        static List<GISField> ReadFields(BinaryReader br, int FieldCount)
        {
            List<GISField> fields = new List<GISField>();
            for (int fieldIndex = 0; fieldIndex < FieldCount; fieldIndex++)
            {
                Type fieldtype = CalTool.IntToType(br.ReadInt32());
                string fieldname = CalTool.ReadString(br);
                fields.Add(new GISField(fieldtype, fieldname));
            }
            return fields;
        }
        static List<GISVertex> ReadMultipleVertexes(BinaryReader br)
        {
            List<GISVertex> vertexes = new List<GISVertex>();
            int vertexCount = br.ReadInt32();
            for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
            {
                vertexes.Add(new GISVertex(br));
            }
            return vertexes;
        }
        static GISAttribute ReadAttributes(List<GISField> fields, BinaryReader br)
        {
            GISAttribute attribute = new GISAttribute();
            for (int i = 0; i < fields.Count; i++)
            {
                Type type = fields[i].DataType;
                if (type.ToString() == "System.Boolean")
                    attribute.AddValue(br.ReadBoolean());
                else if (type.ToString() == "System.Byte")
                    attribute.AddValue(br.ReadByte());
                else if (type.ToString() == "System.Char")
                    attribute.AddValue(br.ReadChar());
                else if (type.ToString() == "System.Decimal")
                    attribute.AddValue(br.ReadDecimal());
                else if (type.ToString() == "System.Double")
                    attribute.AddValue(br.ReadDouble());
                else if (type.ToString() == "System.Single")
                    attribute.AddValue(br.ReadSingle());
                else if (type.ToString() == "System.Int32")
                    attribute.AddValue(br.ReadInt32());
                else if (type.ToString() == "System.Int64")
                    attribute.AddValue(br.ReadInt64());

                else if (type.ToString() == "System.UInt16")
                    attribute.AddValue(br.ReadUInt16());
                else if (type.ToString() == "System.UInt32")
                    attribute.AddValue(br.ReadUInt32());
                else if (type.ToString() == "System.UInt64")
                    attribute.AddValue(br.ReadUInt64());
                else if (type.ToString() == "System.SByte")
                    attribute.AddValue(br.ReadSByte());
                else if (type.ToString() == "System.Int16")
                    attribute.AddValue(br.ReadInt16());
                else if (type.ToString() == "System.String")
                    attribute.AddValue(CalTool.ReadString(br));
            }
            return attribute;

        }
        static void ReadFeatures(Layer layer, BinaryReader br, int featureCount)
        {
            for (int featureIndex = 0; featureIndex < featureCount; featureIndex++)
            {
                GISFeature feature = new GISFeature(null, null);
                if (layer.ShapeType == S.Point)
                {
                    feature.spatialPart = new GISPoint(new GISVertex(br));
                }
                else if (layer.ShapeType == S.Line)
                {
                    feature.spatialPart = new GISLine(ReadMultipleVertexes(br));
                }
                else if (layer.ShapeType == S.Polygon)
                {
                    feature.spatialPart = new GISPolygon(ReadMultipleVertexes(br));
                }
                feature.attributePart = ReadAttributes(layer.Fields, br);
                layer.AddFeature(feature);
            }
        }
        public static Layer ReadFile(string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(fileStream);
            MyFileHeader header = (MyFileHeader)(CalTool.FromBytes(br, typeof(MyFileHeader)));
            S shapeType = (S)Enum.Parse(typeof(S), header.ShapeType.ToString());
            GISMapExtent extent = new GISMapExtent(header.MinX, header.MaxX, header.MinY, header.MaxY);
            string layername = CalTool.ReadString(br);
            List<GISField> fields = ReadFields(br, header.FieldCount);
            Layer layer = new Layer(layername, shapeType, extent, fields);
            ReadFeatures(layer, br, header.FeatureCount);
            br.Close();
            fileStream.Close();
            return layer;
        }

    }

}
