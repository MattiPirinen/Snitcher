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
        public static List<Line> CreateVoronoi(List<Point2d> pts, Polyline boarder, BoundingBox bb)
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

            List<Line> vornoi = CreateLines(vornoiEdges, boarder);
            return vornoi;
        }

        private static List<Line> CreateLines(IEnumerable<Edge> vornoiEdges, Polyline boarder)
        {
            List<double> parameters = new List<double>();
            List<Line> lines = new List<Line>();
            foreach (var edge in vornoiEdges)
            {
                lines.Add(new Line(ToRhinoPoint(edge.Point1), ToRhinoPoint(edge.Point2)));
            }
            Curve boarderc = boarder.ToNurbsCurve();
            int i = 0;
            while (i < lines.Count)
            {
                bool containsStart = boarderc.Contains(lines[i].From,Plane.WorldXY,0.001) == PointContainment.Inside;
                bool containsEnd = boarderc.Contains(lines[i].To, Plane.WorldXY,0.001) == PointContainment.Inside;
                if ((containsEnd && containsStart) || (!containsEnd && !containsStart))
                {
                    lines.RemoveAt(i);
                    continue;
                }
                    
                CurveIntersections ci = Intersection.CurveCurve(lines[0].ToNurbsCurve(), boarderc, 0.001, 0.001);
                if (ci.Count != 0)
                {
                    if (containsStart)
                        lines[i] = new Line(lines[i].From, lines[i].PointAt(ci[0].ParameterA));
                    else
                        lines[i] = new Line(lines[i].PointAt(ci[0].ParameterA), lines[i].To);
                    parameters.Add(ci[0].ParameterB);
                }
                i++;
            }
            parameters.Sort();
            Curve[] curves = boarderc.Split(parameters);
            foreach (var curve in curves)
            {
                curve.TryGetPolyline(out Polyline pl);
                lines.AddRange(pl.GetSegments());
            }
            return lines;
        }

        private static Point3d ToRhinoPoint(delPoint point1)
        {
            return new Point3d(point1.X, point1.Y, 0);
        }
    }
}
