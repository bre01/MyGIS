using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace My.GIS
{
    public class GISDocument
    {
        public List<Layer> Layers = new List<Layer>();
        GISMapExtent _extent;
        public GISMapExtent Extent
        {
            get
            {
                return _extent;
            }
            set
            {
            }
        }
        public Layer GetLayer(string layerName)
        {
            for (int i = 0; i < Layers.Count;)
            {
                if (Layers[i].Name == layerName)
                {
                    return Layers[i];
                }
            }
            return null;
        }
        public Layer AddLayer(string path)
        {
            Layer layer = null;
            string fileType = System.IO.Path.GetExtension(path).ToLower();
            if (fileType == "." + GISConst.SHPFILE)
            {
                ShapefileTools shapefileTools = new ShapefileTools();
                layer = shapefileTools.ReadShapefile(path);
            }
            else if (fileType == "." + GISConst.MYFILE)
            {
                layer = MyFiles.ReadFile(path);
            }
            layer.path = path;
            GetUniqueName(layer);
            Layers.Add(layer);
            UpdateExtent();
            
            return layer;

        }
        public void RemoveLayer(string name)
        {
            Layers.Remove(GetLayer(name));
            UpdateExtent();
        }
        public void GetUniqueName(Layer layer)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < Layers.Count; i++)
            {
                names.Add(Layers[i].Name);
            }
            names.Sort();
            for (int i = 0; i < names.Count; i++)
            {
                if (layer.Name == names[i])
                {
                    layer.Name = names[i] + "1";
                }
            }
        }
        public void UpdateExtent()
        {
            _extent = null;
            if (Layers.Count == 0)
            {
                return;
            }
            _extent = new GISMapExtent(Layers[0].DisplayExtent);
            for (int i = 1; i < Layers.Count; i++)
            {
                _extent.Merge(Layers[i].DisplayExtent);
            }
        }
        public void Draw(Graphics graphics, MapAndClientConverter converter)
        {
            if (Layers.Count == 0) return;
            GISMapExtent displayExtent = converter.GetDisplayExtent();
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].Visible)
                {
                    Layers[i].Draw(graphics, converter, displayExtent);
                }
            }
        }
        public void SwitchLayer(string name1, string name2)
        {
            Layer layer1 = GetLayer(name1);
            Layer layer2 = GetLayer(name2);
            int index1 = Layers.IndexOf(layer1);
            int index2 = Layers.IndexOf(layer2);
            Layers[index1] = layer2;
            Layers[index2] = layer1;
        }
        public void Write(string fileName)
        {
            FileStream fsr = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fsr);
            for (int i = 0; i < Layers.Count; i++)
            {
                CalTool.WriteString(Layers[i].Name, bw);
                bw.Write(Layers[i].DrawAttributeOrNot);
                bw.Write(Layers[i].LabelIndex);
                bw.Write(Layers[i].Selectable);
                bw.Write(Layers[i].Visible);
            }
            bw.Close();
            fsr.Close();
        }
        public void Read(string fileName)
        {
            Layers.Clear();
            FileStream fsr = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fsr);
            while (br.PeekChar() != -1)
            {
                string path = CalTool.ReadString(br);
                Layer layer = AddLayer(path);
                layer.path = path;
                layer.DrawAttributeOrNot = br.ReadBoolean();
                layer.LabelIndex = br.ReadInt32();
                layer.Selectable = br.ReadBoolean();
                layer.Visible = br.ReadBoolean();
            }
            br.Close();
            br.Close();
        }
        public bool IsEmpty()
        {
            return (Layers.Count == 0);
        }
        public void ClearSelection()
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                Layers[i].ClearSelection();
            }
        }
        public SelectResult Select(GISVertex v, MapAndClientConverter converter)
        {
            SelectResult sr = SelectResult.TooFar;
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].Selectable)
                {
                    if (Layers[i].Select(v, converter) == SelectResult.Ok)
                    {
                        sr = SelectResult.Ok;
                    }
                }
            }
            return sr;
        }
        public SelectResult Select(GISMapExtent extent)
        {
            SelectResult sr = SelectResult.TooFar;
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].Selectable)
                {
                    if (Layers[i].Select(extent) == SelectResult.Ok)
                    {
                        sr = SelectResult.Ok;
                    }
                }
            }
            return sr;
        }

    }
}
