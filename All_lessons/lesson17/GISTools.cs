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
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;

namespace My.GIS
{
public static class GISTools
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
        public static DataTable ReadDBF(String dbfFilename)
        {
            System.IO.FileInfo fileInfo = new FileInfo(dbfFilename);
            DataSet dataset = null;
            string constr = @"Provider=Microsoft.Jet.Oledb.4.0;Data Source="
                + fileInfo.DirectoryName
                + ";Extended Properties='DBASE III;'";
            using (OleDbConnection con = new OleDbConnection(constr))
            {
                var sql = "select * from " + fileInfo.Name;
                OleDbCommand cmd = new OleDbCommand(sql, con);
                con.Open();
                dataset = new DataSet();
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                adapter.Fill(dataset);
            }
            return dataset.Tables[0];
        }
        public static List<GISField> ReadFields(DataTable table)
        {
            List<GISField> fields = new List<GISField>();
            foreach (DataColumn column in table.Columns)
            {
                fields.Add(new GISField(column.DataType, column.ColumnName));
            }
            return fields;
        }
        public static GISAttribute ReadAttribute(DataTable table, int rowIndex)
        {
            GISAttribute attribute = new GISAttribute();
            DataRow row = table.Rows[rowIndex];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                attribute.AddValue(row[i]);
            }
            return attribute;

        }

        public static byte[] ToBytes(object c)
        {
            byte[] bytes = new byte[Marshal.SizeOf(c.GetType())];
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(c, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return bytes;
        }
        public static void WriteString(string s, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(StringLength(s));
            byte[] sbytes = Encoding.Default.GetBytes(s);
            binaryWriter.Write(sbytes);
        }
        public static int StringLength(string s)
        {
            int ChineseCount = 0;
            byte[] bytes = new ASCIIEncoding().GetBytes(s);
            foreach (byte b in bytes)
            {
                if (b == 0X3F) ChineseCount++;

            }
            return ChineseCount + bytes.Length;
        }
        public static int TypeToInt(Type type)
        {
            ALLTYPES onetype = (ALLTYPES)Enum.Parse(typeof(ALLTYPES), type.ToString().Replace(".", "_"));
            return (int)onetype;
        }
        public static Object FromBytes(BinaryReader br, Type type)
        {
            byte[] bytes = br.ReadBytes(Marshal.SizeOf(type));
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            Object result = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
            handle.Free();
            return result;
        }
        public static Type IntToType(int index)
        {
            string typeString = Enum.GetName(typeof(ALLTYPES), index);
            typeString = typeString.Replace("_", ".");
            return Type.GetType(typeString);
        }
        public static string ReadString(BinaryReader br)
        {
            int length = br.ReadInt32();
            byte[] sbytes = br.ReadBytes(length);
            return Encoding.Default.GetString(sbytes);
        }
        public static double PointToSegment(GISVertex A, GISVertex B, GISVertex C)
        {
            double dot1 = Dot3Product(A, B, C);
            if (dot1 > 0) return B.GetDistanceThisVToV(C);
            double dot2 = Dot3Product(B, A, C);
            if (dot2 > 0) return A.GetDistanceThisVToV(C);
            double distance = Cross3Product(A, B, C) / A.GetDistanceThisVToV(B);
            return Math.Abs(distance);
        }
        static double Dot3Product(GISVertex A, GISVertex B, GISVertex C)
        {
            GISVertex AB = new GISVertex(B.x - A.x, B.y - A.y);
            GISVertex BC = new GISVertex(C.x - B.x, C.y - B.y);
            return AB.x * BC.x + AB.y * BC.y;
        }
        static double Cross3Product(GISVertex A, GISVertex B, GISVertex C)
        {
            GISVertex AB = new GISVertex(B.x - A.x, B.y - A.y);
            GISVertex AC = new GISVertex(C.x - A.x, C.y - A.y);
            return VectorProduct(AB, AC);
        }
        public static Random rand = new Random();
        public static Color GetRandomColor()
        {
            return Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }

    }
}