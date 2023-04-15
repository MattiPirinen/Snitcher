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
            List<Line> testLines = new List<Line>();
            foreach (var traing in triangulation.ToList())
            {
                testLines.Add(new Line(new Point3d(traing.Vertices[0].X, traing.Vertices[0].Y, 0), new Point3d(traing.Vertices[1].X, traing.Vertices[1].Y, 0)));
                testLines.Add(new Line(new Point3d(traing.Vertices[1].X, traing.Vertices[1].Y, 0), new Point3d(traing.Vertices[2].X, traing.Vertices[2].Y, 0)));
                testLines.Add(new Line(new Point3d(traing.Vertices[2].X, traing.Vertices[2].Y, 0), new Point3d(traing.Vertices[0].X, traing.Vertices[0].Y, 0)));
            }

            var vornoiEdges = voronoi.GenerateEdgesFromDelaunay(triangulation);

            List<Line> vornoi = CreateLines(vornoiEdges, boarder);
            Dictionary<Point3d, List<Line>> connectedLines = CreateConnectedLines(vornoi);
            List<Polyline> cells = CreateCells(connectedLines, new List<Line>(vornoi));


            return testLines;
        }

        private static List<Polyline> CreateCells(Dictionary<Point3d, List<Line>> connectedLines,List<Line> allLines)
        {
            List<Polyline> cells = new List<Polyline>();
            Dictionary<Line, (bool clockwise, bool counterclockwise)> usedLines = new Dictionary<Line, (bool clockwise, bool counterclockwise)>();
            foreach (var line in allLines)
            {
                usedLines.Add(line, (false,false));
            }

            foreach (var item in connectedLines)
            {
                foreach (var line in item.Value)
                {
                    if (!usedLines[line].clockwise)
                    {
                        cells.Add(CreateCell(connectedLines, line, true));
                        usedLines[line] = (true, usedLines[line].counterclockwise);
                    }

                    if (!usedLines[line].clockwise)
                    {
                        cells.Add(CreateCell(connectedLines, line, false));
                        usedLines[line] = (usedLines[line].clockwise, true);
                    }
                }
            }
            return cells;

        }
        



        private static Polyline CreateCell(Dictionary<Point3d, List<Line>> connectedLines, Line line, bool v)
        {
            throw new NotImplementedException();
        }

        private static Dictionary<Point3d,List<Line>> CreateConnectedLines(List<Line> vornoi)
        {
            Dictionary<Point3d,List<Line>> connectedLines =  new Dictionary<Point3d,List<Line>>() ;

            foreach (var item in vornoi)
            {
                if (connectedLines.ContainsKey(item.From))
                    connectedLines[item.From].Add(item);
                else
                    connectedLines.Add(item.From, new List<Line>() { item});
                if (connectedLines.ContainsKey(item.To))
                    connectedLines[item.To].Add(item);
                else
                    connectedLines.Add(item.To, new List<Line>() { item});
            }

            return connectedLines;
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
                    if ((!containsEnd && !containsStart))
                    {
                        lines.RemoveAt(i);
                        continue;
                    }
                    else
                    {
                        i++;
                        continue;
                    }
                }
                Curve lineCurve = lines[i].ToNurbsCurve();
                CurveIntersections ci = Intersection.CurveCurve(lineCurve, boarderc, 0.001, 0.001);
                if (ci.Count != 0)
                {
                    if (containsStart)
                        lines[i] = new Line(lines[i].From, lineCurve.PointAt(ci[0].ParameterA));
                    else
                        lines[i] = new Line(lineCurve.PointAt(ci[0].ParameterA), lines[i].To);
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
