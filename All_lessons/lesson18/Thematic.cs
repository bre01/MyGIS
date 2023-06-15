using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.GIS
{
    public class GISThematic
    {
        public Color OutsideColor;
        public int Size;
        public Color InsideColor;
        public GISThematic(Color outsideColor, int size, Color insideColor)
        {
            UpdateColor(outsideColor, size, insideColor);
        }
        public GISThematic(S shapeType)
        {
            if (shapeType == S.Point)
            {
                UpdateColor(GISTools.GetRandomColor(), GISConst.PointSize, GISTools.GetRandomColor());
            }
            else if (shapeType == S.Line)
            {
                UpdateColor(GISTools.GetRandomColor(), GISConst.LineWidth, GISTools.GetRandomColor());
            }
            else if(shapeType== S.Polygon)
            {
                UpdateColor(GISTools.GetRandomColor(), GISConst.PolygonBoundaryWidth, GISTools.GetRandomColor()); 
            }

        }
        public void UpdateColor(Color outsideColor, int size, Color insideColor)
        {

            OutsideColor = outsideColor;
            Size = size;
            InsideColor = insideColor;
        }
    }

}
