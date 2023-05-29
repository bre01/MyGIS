using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.GIS
{
    public class GISDocument
    {
        public List<Layer> Layers = new List<Layer>();
        GISMapExtent _extent;
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
    }
}
