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
        public static List<Polyline> CreateVoronoi(List<Point2d> pts)
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

            List<Polyline> vornoi = CreatePolylines(vornoiEdges);
        }

        private static List<Polyline> CreatePolylines(IEnumerable<Edge> vornoiEdges)
        {
            List<Polyline> polylines = new List<Polyline>();
            foreach (var edge in vornoiEdges)
            {
                edge.
            }
        }
    }
}
