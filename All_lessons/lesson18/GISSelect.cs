using System;
//using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace My.GIS
{
public class GISSelect
    {
        public GISFeature SelectedFeature = null;
        public List<GISFeature> SelectedFeatures = new List<GISFeature>();
        public SelectResult Select(GISVertex vertex, List<GISFeature> features, S shapeType, MapAndClientConverter converter)
        {
            if (features.Count == 0) { return SelectResult.Ok; }
            GISMapExtent minSelectExtent = BuildExtent(vertex, converter);
            switch (shapeType)
            {
                case S.Point: return SelectPoint(vertex, features, converter, minSelectExtent);
                case S.Line: return SelectLine(vertex, features, converter, minSelectExtent);
                case S.Polygon: return SelectPolygon(vertex, features, converter, minSelectExtent);
            }
            return SelectResult.UnknownType;
        }
        public SelectResult Select(GISMapExtent extent, List<GISFeature> features)
        {
            SelectedFeatures.Clear();
            for (int i = 0; i < features.Count; i++)
            {
                if (extent.Include(features[i].spatialPart.mapExtent))
                {
                    SelectedFeatures.Add(features[i]);
                }
            }
            return (SelectedFeatures.Count > 0) ? SelectResult.Ok : SelectResult.TooFar;

        }
        public GISMapExtent BuildExtent(GISVertex vertex, MapAndClientConverter converter)
        {
            Point p0 = converter.ToScreenPoint(vertex);
            Point p1 = new Point(p0.X + (int)GISConst.MinScreenDistance, p0.Y + (int)GISConst.MinScreenDistance);
            Point p2 = new Point(p0.X - (int)GISConst.MinScreenDistance, p0.Y - (int)GISConst.MinScreenDistance);
            GISVertex gp1 = converter.ToMapVertex(p1);
            GISVertex gp2 = converter.ToMapVertex(p2);
            return new GISMapExtent(gp1.x, gp2.x, gp1.y, gp2.y);
        }
        public SelectResult SelectPoint(GISVertex vertex, List<GISFeature> features, MapAndClientConverter converter, GISMapExtent MinSelectExtent)
        {
            double resultDistance = double.MaxValue;
            int id = -1;
            for (int i = 0; i < features.Count; i++)
            {
                if (MinSelectExtent.IntersectOrNot(features[i].spatialPart.mapExtent) == false) continue;
                GISPoint point = (GISPoint)(features[i].spatialPart);
                double distance = point.GetDistanceThisPointToVertex(vertex);
                if (distance < resultDistance)
                {
                    resultDistance = distance;
                    id = i;
                }
            }
            if (id == -1)
            {
                SelectedFeature = null;
                return SelectResult.TooFar;
            }
            else
            {
                double screenDistance = converter.ToScreenDistance(vertex, features[id].spatialPart.centroid);
                if (screenDistance <= GISConst.MinScreenDistance)
                {
                    SelectedFeature = features[id];
                    return SelectResult.Ok;
                }
                else
                {
                    SelectedFeature = null;
                    return SelectResult.TooFar;
                }
            }
        }
        public SelectResult SelectLine(GISVertex vertex, List<GISFeature> features, MapAndClientConverter converter, GISMapExtent MinSelectExtent)
        {
            double resultDistance = double.MaxValue;
            int id = -1;
            for (int i = 0; i < features.Count; i++)
            {
                if (MinSelectExtent.IntersectOrNot(features[i].spatialPart.mapExtent) == false) continue;
                GISLine line = (GISLine)(features[i].spatialPart);
                double distance = line.GetDistanceFromThisToVertex(vertex);
                if (distance < resultDistance)
                {
                    resultDistance = distance;
                    id = i;
                }
            }

            if (id == -1)
            {
                SelectedFeature = null;
                return SelectResult.TooFar;
            }
            else
            {
                double screenDistance = converter.ToScreenDistance(resultDistance);
                if (screenDistance <= GISConst.MinScreenDistance)
                {
                    SelectedFeature = features[id];
                    return SelectResult.Ok;
                }
                else
                {
                    SelectedFeature = null;
                    return SelectResult.TooFar;

                }
            }
        }
        public SelectResult SelectPolygon(GISVertex vertex, List<GISFeature> features, MapAndClientConverter converter, GISMapExtent MinSelectExtent)
        {
            SelectedFeatures.Clear();
            for (int i = 0; i < features.Count; i++)
            {
                if (MinSelectExtent.IntersectOrNot(features[i].spatialPart.mapExtent) == false) continue;
                GISPolygon polygon = (GISPolygon)(features[i].spatialPart);
                if (polygon.Include(vertex))
                {
                    SelectedFeatures.Add(features[i]);
                }
            }
            return (SelectedFeatures.Count > 0) ? SelectResult.Ok : SelectResult.TooFar;
        }



    }
}