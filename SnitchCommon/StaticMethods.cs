using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelaunayVoronoi;
using Rhino.Geometry;
using delPoint = DelaunayVoronoi.Point;

namespace SnitchCommon
{
    public static class StaticMethods
    {
        public static List<Line> CreateVoronoi(List<Point2d> pts)
        {
            DelaunayTriangulator delaunay = new DelaunayTriangulator();
            Voronoi voronoi = new Voronoi();
            List<delPoint> delanayPoints = new List<delPoint>();
            foreach (var pt in pts)
            {
                delanayPoints.Add(new delPoint(pt.X, pt.Y));
            }
            var triangulation = delaunay.BowyerWatson(delanayPoints);
            var vornoiEdges = voronoi.GenerateEdgesFromDelaunay(triangulation);

            List<Line> vornoi = CreatePolylines(vornoiEdges);
            return vornoi;
        }

        private static List<Line> CreatePolylines(IEnumerable<Edge> vornoiEdges)
        {
            List<Line> polylines = new List<Line>();
            foreach (var edge in vornoiEdges)
            {
                polylines.Add(new Line(ToRhinoPoint(edge.Point1), ToRhinoPoint(edge.Point2)));
            }
            return polylines;
        }

        private static Point3d ToRhinoPoint(delPoint point1)
        {
            return new Point3d(point1.X, point1.Y, 0);
        }
    }
}
