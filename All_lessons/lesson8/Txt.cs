using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My.GIS
{
    public class Txt

    {

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct MyFileHeader
        {
            public double MinX, MinY, MaxX, MaxY;
            public int FeatureCount, ShapeType, FieldCount;
        }
        public static void WriteTxt(Layer layer, string filename)
        {

            using (StreamWriter streamWriter = new StreamWriter(filename))
            {
                WriteHeaderTxt(layer, streamWriter);
                WriteLayerNameTxt(layer.Name, streamWriter);
                WriteFieldsTxt(layer.Fields, streamWriter);
                WriteFeaturesTxt(layer, streamWriter);
            }
        }
        public static void WriteHeaderTxt(Layer layer, StreamWriter streamWriter)
        {
            streamWriter.WriteLine(layer.Extent.minX());
            streamWriter.WriteLine(layer.Extent.maxX());
            streamWriter.WriteLine(layer.Extent.minY());
            streamWriter.WriteLine(layer.Extent.maxY());
            streamWriter.WriteLine(layer.FeatureCount());
            streamWriter.WriteLine((int)layer.ShapeType);
            streamWriter.WriteLine(layer.Fields.Count());
        }
        public static void WriteLayerNameTxt(string name, StreamWriter sw)
        {
            sw.WriteLine(name);
        }
        public static void WriteFieldsTxt(List<GISField> fields, StreamWriter sw)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                sw.WriteLine(fields[i].DataType);
                sw.WriteLine(fields[i].Name);
            }
        }
        public static void WriteFeaturesTxt(Layer layer, StreamWriter sw)
        {
            for (int featureIndex = 0; featureIndex < layer.FeatureCount(); featureIndex++)
            {
                GISFeature feature = layer.GetFeature(featureIndex);
                if (layer.ShapeType == S.Point)
                {
                    ((GISPoint)feature.spatialPart).centroid.WriteVertex(sw);
                }
                else if (layer.ShapeType == S.Line)
                {
                    GISLine line = (GISLine)(feature.spatialPart);
                    WriteMultipleVertexes(line.Vertexes, sw);

                }
                else if (layer.ShapeType == S.Polygon)
                {
                    GISPolygon polygon = (GISPolygon)(feature.spatialPart);
                    WriteMultipleVertexes(polygon.Vertexes, sw);
                }
                WriteAttributes(feature.attributePart, sw);
            }
        }
        static void WriteMultipleVertexes(List<GISVertex> vertexes, StreamWriter sw)
        {
            sw.WriteLine(vertexes.Count());
            for (int vertexIndex = 0; vertexIndex < vertexes.Count(); vertexIndex++)
            {
                vertexes[vertexIndex].WriteVertex(sw);
            }
        }
        static void WriteAttributes(GISAttribute attribute, StreamWriter sw)
        {
            for (int i = 0; i < attribute.ValueCount(); i++)
            {
                var value = attribute.GetValue(i);
                sw.WriteLine(value);
            }
        }
        static List<GISVertex> ReadMultipleVertexes(StreamReader sr)
        {
            List<GISVertex> vertexes = new List<GISVertex>();
            int vertexCount = Convert.ToInt32(sr.ReadLine());
            for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
            {
                vertexes.Add(new GISVertex(sr));
            }
            return vertexes;
        }
        static GISAttribute ReadAttributes(List<GISField> fields, StreamReader sr)
        {
            GISAttribute attribute = new GISAttribute();
            for (int i = 0; i < fields.Count; i++)
            {
                Type type = fields[i].DataType;
                //attribute.AddValue(CastObject < type> (sr.ReadLine()));
                attribute.AddValue(Convert.ChangeType(sr.ReadLine(), type));
            }
            return attribute;
        }

        public static Layer ReadTxt(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                MyFileHeader header = ReadHeaderTxt(sr);
                string name = ReadLayerNameTxt(sr);

                List<GISField> fields = ReadFieldsTxt(sr, header.FieldCount);
                S shapeType = (S)Enum.Parse(typeof(S), header.ShapeType.ToString());
                Layer layer = new Layer(name, shapeType,
                    new GISMapExtent(header.MinX, header.MaxX, header.MinY, header.MaxY),
                    fields);

                ReadFeaturesTxt(layer, sr, header.FeatureCount);


                return layer;

            }

        }
        public static MyFileHeader ReadHeaderTxt(StreamReader sr)
        {
            MyFileHeader header = new MyFileHeader();
            header.MinX = (Convert.ToDouble(sr.ReadLine()));
            header.MaxX = (Convert.ToDouble(sr.ReadLine()));
            header.MinY = (Convert.ToDouble(sr.ReadLine()));
            header.MaxY = (Convert.ToDouble(sr.ReadLine()));
            header.FeatureCount = Convert.ToInt32(sr.ReadLine());
            header.ShapeType = Convert.ToInt32(sr.ReadLine());
            header.FieldCount = Convert.ToInt32(sr.ReadLine());
            return header;
        }
        public static String ReadLayerNameTxt(StreamReader sr)
        {
            return (sr.ReadLine());
        }
        public static List<GISField> ReadFieldsTxt(StreamReader sr, int FieldCount)
        {
            List<GISField> fields = new List<GISField>();
            for (int fieldIndex = 0; fieldIndex < FieldCount; fieldIndex++)
            {
                fields.Add(new GISField(Type.GetType(sr.ReadLine()), sr.ReadLine()));
            }
            return fields;
        }
        public static void ReadFeaturesTxt(Layer layer, StreamReader sr, int featureCount)
        // objects are by default passed by ref  ,so layer is passed by ref
        {
            for (int featureIndex = 0; featureIndex < featureCount; featureIndex++)
            {
                GISFeature feature = new GISFeature(null, null);
                if (layer.ShapeType == S.Point)
                {
                    feature.spatialPart = new GISPoint(new GISVertex(sr));
                }
                else if (layer.ShapeType == S.Line)
                {
                    feature.spatialPart = new GISLine(ReadMultipleVertexes(sr));
                }
                else if (layer.ShapeType == S.Polygon)
                {
                    feature.spatialPart = new GISPolygon(ReadMultipleVertexes(sr));
                }
                feature.attributePart = ReadAttributes(layer.Fields, sr);
                layer.AddFeature(feature);
            }



        }

    }
}
