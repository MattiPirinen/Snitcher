using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelaunayVoronoi;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using delPoint = DelaunayVoronoi.Point;

namespace SnitchCommon
{
    public static class StaticMethods
    {
        public static VoronoiModel CreateVoronoi(List<Point2d> pts, Polyline boarder, BoundingBox bb)
        {
            

            DelaunayTriangulator delaunay = new DelaunayTriangulator();

            Voronoi voronoi = new Voronoi();
            List<delPoint> delanayPoints = new List<delPoint>();
            
            foreach (var pt in pts)
            {
                delanayPoints.Add(new delPoint(pt.X, pt.Y));
            }

            List<delPoint> delBoarder = new List<delPoint>
            {
                new delPoint(bb.Min.X - 100000, bb.Min.Y - 100000),
                new delPoint(bb.Max.X + 100000, bb.Min.Y - 100000),
                new delPoint(bb.Max.X + 100000, bb.Max.Y + 100000),
                new delPoint(bb.Min.X - 100000, bb.Max.Y + 100000)
            };

            delaunay.GenerateBoarder(delBoarder);


            var triangulation = delaunay.BowyerWatson(delanayPoints);
            var vornoiEdges = voronoi.GenerateEdgesFromDelaunay(triangulation);
            VoronoiModel model = new VoronoiModel();
            model.CreateLines(vornoiEdges, boarder);
            model.CreateNodes();
            model.CreateCells();



            return model;
        }

        public static Point3d ToRhinoPoint(delPoint point1)
        {
            return new Point3d(point1.X, point1.Y, 0);
        }
    }
}
