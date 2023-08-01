using DelaunayVoronoi;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using delPoint = DelaunayVoronoi.Point;

namespace VoronoiExtension
{
    public class VoronoiModel
    {
        public VoronoiPointCloud NodeCloud {get;set;} = new VoronoiPointCloud();
        public List<VoronoiLine> Lines { get; set; } = new List<VoronoiLine>();

        public List<VoronoiCell> Cells { get; set; }  = new List<VoronoiCell>();



        public void ArrangeLinesInNodes()
        {
            foreach (var node in NodeCloud.Nodes)
            {
                node.ArrangeLines();
            }
        }

        public void CreateCells()
        {
            Cells = new List<VoronoiCell>();
            foreach (var node in NodeCloud.Nodes)
            {
                foreach (var line in node.VoronoiLines)
                {
                    if (line.IsEdgeLine)
                        continue;
                    else if (line.UsedClockWise && line.UsedCounterClockWise) continue;
                    if (!line.UsedClockWise)
                        CreateCell(line,node, true);
                    if (!line.UsedCounterClockWise)
                        CreateCell(line, node, false);
                }
            }
            RemoveDuplicateCells();
        }

        private void RemoveDuplicateCells()
        {
            List<VoronoiCell> pls = new List<VoronoiCell>();
            PointCloud pl = new PointCloud();
            foreach (var cell in Cells)
            {
                Point3d pt = AreaMassProperties.Compute(cell.Perimeter.ToNurbsCurve()).Centroid;
                int i = pl.ClosestPoint(pt);
                if (i == -1)
                {
                    pls.Add(cell);
                }
                else
                {
                    if ((pl[i].Location - pt).Length > 0.001)
                        pls.Add(cell);
                }
                pl.Add(pt);

            }
            Cells = pls;
        }

        private void CreateCell(VoronoiLine line, VoronoiNode startNode, bool clockWise)
        {
            try
            {
                List<Curve> cellLines = new List<Curve>();
                if (GetLineAndMoveNext(line, cellLines, startNode, startNode, clockWise))
                {
                    Curve c = Curve.JoinCurves(cellLines)[0];
                    c.TryGetPolyline(out Polyline pl);
                    Point3d pt =pl.CenterPoint();
                    int index = NodeCloud.ClosestPoint(pt);

                    Cells.Add(new VoronoiCell() { Perimeter = pl, Node = NodeCloud.Nodes[index] });
                }
                
            }
            catch { }


        }

        private bool GetLineAndMoveNext(VoronoiLine line, List<Curve> cellLines, VoronoiNode cellStartNode, VoronoiNode prevNode, bool clockWise)
        {
            //if (clockWise && line.UsedClockWise) return true;
            cellLines.Add(line.Line.ToNurbsCurve());
            /*
            if (clockWise)
                line.UsedClockWise = true;
            else
                line.UsedCounterClockWise= true;
            */
            if (line.StartNode == prevNode)
            {
                if (line.EndNode == cellStartNode)
                    return true;
                if (clockWise)
                    return GetLineAndMoveNext(line.EndNodeNextLineClockWise,cellLines,cellStartNode,line.EndNode,clockWise);
                else
                    return GetLineAndMoveNext(line.EndNodeNextLineCounterClockWise, cellLines, cellStartNode, line.EndNode, clockWise);
            }
            else if (line.EndNode == prevNode)
            {
                if (line.StartNode == cellStartNode)
                    return true;
                if (clockWise)
                    return GetLineAndMoveNext(line.StartNodeNextLineClockWise, cellLines, cellStartNode, line.StartNode, clockWise);
                else
                    return GetLineAndMoveNext(line.StartNodeNextLineCounterClockWise, cellLines, cellStartNode, line.StartNode, clockWise);
            }
            return true;
        }

        public void CreateNodes()
        {
            Dictionary<Point3d, VoronoiNode> voronoiNodes = new Dictionary<Point3d, VoronoiNode>();

            foreach (var item in Lines)
            {
                Point3d roundedPtStart = new Point3d(Math.Round(item.Line.From.X, 3), Math.Round(item.Line.From.Y, 3), Math.Round(item.Line.From.Z, 3));

                if (voronoiNodes.ContainsKey(roundedPtStart))
                {
                    voronoiNodes[roundedPtStart].VoronoiLines.Add(item);
                    item.StartNode = voronoiNodes[roundedPtStart];
                }
                else
                {
                    VoronoiNode node = new VoronoiNode();
                    node.VoronoiLines.Add(item);
                    node.Location = roundedPtStart;
                    item.StartNode = node;
                    voronoiNodes.Add(roundedPtStart, node);
                }

                Point3d roundedPtEnd = new Point3d(Math.Round(item.Line.To.X, 3), Math.Round(item.Line.To.Y, 3), Math.Round(item.Line.To.Z, 3));

                if (voronoiNodes.ContainsKey(roundedPtEnd))
                {
                    voronoiNodes[roundedPtEnd].VoronoiLines.Add(item);
                    item.EndNode = voronoiNodes[roundedPtEnd];
                }
                else
                {
                    VoronoiNode node = new VoronoiNode();
                    node.VoronoiLines.Add(item);
                    node.Location = roundedPtEnd;
                    item.EndNode = node;
                    voronoiNodes.Add(roundedPtEnd, node);
                }
            }
            VoronoiPointCloud pl = new VoronoiPointCloud();

            List<VoronoiNode> nodes = voronoiNodes.Values.ToList();
            for (int i = 0; i < nodes.Count; i++)
            {
                pl.Add(nodes[i].Location);
                pl.Nodes.Add(nodes[i]);
            }
            NodeCloud = pl;
            ArrangeLinesInNodes();
        }

        public void CreateLines(IEnumerable<Edge> vornoiEdges, Polyline boarder)
        {
            List<double> parameters = new List<double>();
            List<VoronoiLine> lines = new List<VoronoiLine>();
            foreach (var edge in vornoiEdges)
            {
                Line l = new Line(ToRhinoPoint(edge.Point1), ToRhinoPoint(edge.Point2));
                if (l.Length > 0.0001)
                    lines.Add(new VoronoiLine(l));
            }
            
            Curve boarderc = boarder.ToNurbsCurve();
            int i = 0;
            while (i < lines.Count)
            {
                bool containsStart = boarderc.Contains(lines[i].Line.From, Plane.WorldXY, 0.001) == PointContainment.Inside;
                bool containsEnd = boarderc.Contains(lines[i].Line.To, Plane.WorldXY, 0.001) == PointContainment.Inside;
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
                Curve lineCurve = lines[i].Line.ToNurbsCurve();
                CurveIntersections ci = Intersection.CurveCurve(lineCurve, boarderc, 0.001, 0.001);
                if (ci.Count != 0)
                {
                    if (containsStart)
                        lines[i] = new VoronoiLine(new Line(lines[i].Line.From, lineCurve.PointAt(ci[0].ParameterA)));
                    else
                        lines[i] = new VoronoiLine(new Line(lineCurve.PointAt(ci[0].ParameterA), lines[i].Line.To));
                    parameters.Add(ci[0].ParameterB);
                }
                i++;
            }
            parameters.Sort();
            Curve[] curves = boarderc.Split(parameters);
            foreach (var curve in curves)
            {
                curve.TryGetPolyline(out Polyline pl);
                foreach (Line line in pl.GetSegments())
                {
                    lines.Add(new VoronoiLine(line) { IsEdgeLine= true});
                }
            }
            
            Lines = lines;
        }

        public static VoronoiModel CreateVoronoi(List<Point2d> pts, Polyline boarder)
        {
            PointCloud pointl = new PointCloud();
            for (int i = 0; i < pts.Count; i++)
            {
                pointl.Add(new Point3d(pts[i].X, pts[i].Y, 0));
            }
            BoundingBox bb = pointl.GetBoundingBox(false);

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
