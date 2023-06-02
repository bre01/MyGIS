using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.GIS
{

    public class Layer
    {

        /// <naming_convention>
        /// if it's PascalCase, it's public member (of the class)
        /// if it's _camelCase, it's private or internal member (of the class)
        /// if it's camelCase,  it's local variable or parameter (of the method)
        /// </naming_convention>
        public string Name;
        public GISMapExtent DisplayExtent;
        public GISMapExtent OriginalExtent = null;
        public bool DrawAttributeOrNot = false;
        public int LabelIndex;
        public S ShapeType;
        private List<GISFeature> _features = new List<GISFeature>();
        public List<GISField> Fields;
        public List<GISFeature> Selection = new List<GISFeature>();
        public bool Selectable = true;
        public bool Visible = true;
        public string path = "";


        public Layer(string name, S shapeType, GISMapExtent extent)
        {
            this.Name = name;
            ShapeType = shapeType;
            if (OriginalExtent == null)
                OriginalExtent = extent;
            DisplayExtent = extent;
            Fields = new List<GISField>();
        }
        public Layer(string name, S shapeType, GISMapExtent extent, List<GISField> fields)
        {
            Name = name;
            ShapeType = shapeType;
            if (OriginalExtent == null)
                OriginalExtent = extent;
            DisplayExtent = extent;
            Fields = fields;
        }
        public SelectResult Select(GISVertex vertex, MapAndClientConverter converter)
        {
            GISSelect gs = new GISSelect();
            SelectResult sr = gs.Select(vertex, _features, ShapeType, converter);
            if (sr == SelectResult.Ok)
            {
                if (ShapeType == S.Polygon)
                {
                    for (int i = 0; i < gs.SelectedFeatures.Count; i++)
                    {
                        if (gs.SelectedFeatures[i].Selected == false)
                        {
                            gs.SelectedFeatures[i].Selected = true;
                            Selection.Add(gs.SelectedFeatures[i]);
                        }
                    }
                }
                else
                {
                    if (gs.SelectedFeature.Selected == false)
                    {
                        gs.SelectedFeature.Selected = true;
                        Selection.Add(gs.SelectedFeature);
                    }
                }
            }
            return sr;
        }
        public void ClearSelection()
        {
            for (int i = 0; i < Selection.Count; i++)
                Selection[i].Selected = false;
            Selection.Clear();
        }
        public void Draw(Graphics graphics, MapAndClientConverter converter)
        {
            GISMapExtent extent = converter.GetDisplayExtent();
            for (int i = 0; i < _features.Count; i++)
            {
                if (extent.IntersectOrNot(_features[i].spatialPart.mapExtent))
                {

                    _features[i].Draw(graphics, converter, this.DrawAttributeOrNot, this.LabelIndex);
                }

            }
        }
        public void Draw(Graphics graphics, MapAndClientConverter converter, GISMapExtent extent)
        {
            for (int i = 0; i < _features.Count; i++)
            {
                //if (extent.IntersectOrNot(_features[i].spatialPart.mapExtent))
                //{
                    _features[i].Draw(graphics, converter, this.DrawAttributeOrNot, this.LabelIndex);
                //}
            }
        }
        public void AddFeature(GISFeature feature)
        {
            if (_features.Count == 0)
            {
                feature.ID = 0;
            }
            else feature.ID = _features[_features.Count - 1].ID + 1;
            _features.Add(feature);
        }
        public int FeatureCount()
        {
            return _features.Count;
        }
        public GISFeature GetFeature(int i)
        {
            return _features[i];
        }
        public List<GISFeature> GetAllFeatures()
        {
            return _features;
        }
        public void AddSelectedFeatureByID(int id)
        {
            GISFeature feature = GetFeatureByID(id);
            feature.Selected = true;
            Selection.Add(feature);
        }
        public GISFeature GetFeatureByID(int id)
        {
            foreach (GISFeature feature in _features)
            {
                if (feature.ID == id)
                {
                    return feature;
                }
            }
            return null;
        }
        public SelectResult Select(GISMapExtent extent)
        {
            GISSelect gs = new GISSelect();
            SelectResult sr = gs.Select(extent, _features);
            if (sr == SelectResult.Ok)
            {
                for (int i = 0; i < gs.SelectedFeatures.Count; i++)
                {
                    if (gs.SelectedFeatures[i].Selected == false)
                    {
                        gs.SelectedFeatures[i].Selected = true;
                        Selection.Add(gs.SelectedFeatures[i]);
                    }
                }
            }
            return sr;
        }


    }
}
