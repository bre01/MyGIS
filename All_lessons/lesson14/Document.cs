using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.GIS
{
    public class Document
    {
        public List<Layer> Layers=new List<Layer>();
        GISMapExtent _extent;
    }
}
