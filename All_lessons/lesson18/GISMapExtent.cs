using System;
//using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace My.GIS
{
public class GISMapExtent
    {
        //map coordinates is the real coordinates
        public GISVertex MapBottomLeft;
        public GISVertex MapUpRight;
        public GISVertex DisplayBL;
        public GISVertex DisplayUR;
        public void CopyExtent(GISMapExtent extent)
        {
            MapUpRight.CopyVertex(extent.MapUpRight);
            MapBottomLeft.CopyVertex(extent.MapBottomLeft);
        }
        public bool Include(GISMapExtent extent)
        {
            return (maxX() >= extent.maxX() && minX() <= extent.minX()
                && maxY() >= extent.maxY() && minY() <= extent.minY());
        }
        public GISMapExtent(GISVertex bottomLeft, GISVertex upRight)
        {
            MapBottomLeft = bottomLeft;
            MapUpRight = upRight;
        }
        public GISMapExtent(GISMapExtent extent)
        {
            MapUpRight = new GISVertex(extent.MapUpRight);
            MapBottomLeft = new GISVertex(extent.MapBottomLeft);

        }
        public GISMapExtent(double x1, double x2, double y1, double y2)
        {
            //so the order of the parameter doesn't matter
            //we use Math.Max() to determine which one is RightUp or BottomLeft
            MapUpRight = new GISVertex(Math.Max(x1, x2), Math.Max(y1, y2));
            MapBottomLeft = new GISVertex(Math.Min(x1, x2), Math.Min(y1, y2));
        }
        public void Merge(GISMapExtent extent)
        {
            MapUpRight.x = Math.Max(MapUpRight.x, extent.MapUpRight.x);
            MapUpRight.y = Math.Max(MapUpRight.y, extent.MapUpRight.y);
            MapBottomLeft.x = Math.Min(MapBottomLeft.x, extent.MapBottomLeft.x);
            MapBottomLeft.y = Math.Min(MapBottomLeft.y, extent.MapBottomLeft.y);
        }
        // these all all properties, which I didn'e even notice, I'm so stupid
        public double minX() { return MapBottomLeft.x; }
        public double MinX
        {
            get
            { return MapBottomLeft.x; }

            set
            {

            }
        }
        public double MaxX
        {
            get
            {
                return MapUpRight.x;
            }
            set
            {

            }
        }
        public double MinY
        {
            get { return MapBottomLeft.y; }
            set
            {
            }
        }
        public double MaxY
        {
            get { return MapUpRight.y; }
            set { }
        }
        public double Width
        {
            get { return MapUpRight.x - MapBottomLeft.x; }
            set { }
        }
        public double Height
        {
            get { return MapUpRight.y - MapBottomLeft.y; }
            set { }
        }
        public double maxX() { return MapUpRight.x; }
        public double minY() { return MapBottomLeft.y; }
        public double maxY() { return MapUpRight.y; }
        public double width() { return MapUpRight.x - MapBottomLeft.x; }
        public double height() { return MapUpRight.y - MapBottomLeft.y; }
        double zoomFactor = 2;
        double movingFactor = 0.25;
        //read only property to represent  map extent using bottom-left and up-right
        /*public double minX { get { return mapBottomLeft.y; } }
        public double minY { get { return mapBottomLeft.y; } }
        public double maxY { get { return mapUpRight.y; } }
        public double maxX { get { return mapUpRight.x; } }*/
        public GISVertex GetCenter()
        {
            return new GISVertex((MapUpRight.x + MapBottomLeft.x) / 2, (MapUpRight.y + MapBottomLeft.y) / 2);
        }

        public void ChangeExtent(GISMapActions action)
        {
            double newMapMinX = MapBottomLeft.x;
            double newMapMinY = MapBottomLeft.y;
            double newMapMaxX = MapUpRight.x;
            double newMapMaxY = MapUpRight.y;
            switch (action)
            {
                //min is bottom left 
                //max is up right
                case GISMapActions.zoomin:
                    newMapMinX = ((minX() + maxX()) - width() / zoomFactor) / 2;
                    newMapMinY = ((minY() + maxY()) - height() / zoomFactor) / 2;
                    newMapMaxX = ((minX() + maxX()) + width() / zoomFactor) / 2;
                    newMapMaxY = ((minY() + maxY()) + height() / zoomFactor) / 2;
                    break;
                case GISMapActions.zoomout:
                    newMapMinX = ((minX() + maxX()) - width() * zoomFactor) / 2;
                    newMapMinY = ((minY() + maxY()) - height() * zoomFactor) / 2;
                    newMapMaxX = ((minX() + maxX()) + width() * zoomFactor) / 2;
                    newMapMaxY = ((minY() + maxY()) + height() * zoomFactor) / 2;
                    break;
                case GISMapActions.moveViewDown:
                    newMapMinY = minY() - height() * movingFactor;
                    newMapMaxY = maxY() - height() * movingFactor;
                    break;
                case GISMapActions.moveViewUp:
                    newMapMinY = minY() + height() * movingFactor;

                    newMapMaxY = maxY() + height() * movingFactor;
                    break;
                case GISMapActions.moveViewRight:
                    newMapMinX = minX() + width() * movingFactor;
                    newMapMaxX = maxX() + width() * movingFactor;
                    break;
                case GISMapActions.moveViewLeft:
                    newMapMaxX = maxX() - width() * movingFactor;
                    newMapMinX = minX() - width() * movingFactor;
                    break;

            }
            MapUpRight.x = newMapMaxX;
            MapUpRight.y = newMapMaxY;
            MapBottomLeft.x = newMapMinX;
            MapBottomLeft.y = newMapMinY;
        }
        public bool IntersectOrNot(GISMapExtent extent)
        {
            return !(maxX() < extent.minX() ||
                    minX() > extent.maxX() ||
                    maxY() < extent.minY() ||
                    minY() > extent.maxY());
        }

    }
}