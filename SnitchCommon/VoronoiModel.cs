using DelaunayVoronoi;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class VoronoiModel
    {
        public List<VoronoiNode> Nodes {get;set;} = new List<VoronoiNode>();
        public List<VoronoiLine> Lines { get; set; } = new List<VoronoiLine>();

        public List<Polyline> Cells { get; set; } 

        public void ArrangeLinesInNodes()
        {
            foreach (var node in Nodes)
            {
                node.ArrangeLines();
            }
        }

        public void CreateCells()
        {
            Cells = new List<Polyline>();
            foreach (var node in Nodes)
            {
                foreach (var line in node.VoronoiLines)
                {
                    if (line.IsEdgeLine)
                        continue;
                    else if (line.UsedClockWise && line.UsedCounterClockWise) continue;
                    if (!line.UsedClockWise)
                        CreateCell(line,node, true);
                }
            }

        }

        private void CreateCell(VoronoiLine line, VoronoiNode startNode, bool clockWise)
        {
            try
            {
                List<Curve> cellLines = new List<Curve>();
                GetLineAndMoveNext(line, cellLines, startNode, startNode, clockWise);
                Curve c = Curve.JoinCurves(cellLines)[0];
                c.TryGetPolyline(out Polyline pl);
                Cells.Add(pl);
            }
            catch { }


        }

        private void GetLineAndMoveNext(VoronoiLine line, List<Curve> cellLines, VoronoiNode cellStartNode, VoronoiNode prevNode, bool clockWise)
        {
            cellLines.Add(line.Line.ToNurbsCurve());
            if (clockWise)
                line.UsedClockWise = true;
            else
                line.UsedCounterClockWise= true;
            if (line.StartNode == prevNode)
            {
                if (line.EndNode == cellStartNode)
                    return;
                if (clockWise)
                    GetLineAndMoveNext(line.EndNodeNextLineClockWise,cellLines,cellStartNode,line.EndNode,clockWise);
                else
                    GetLineAndMoveNext(line.EndNodeNextLineCounterClockWise, cellLines, cellStartNode, line.EndNode, clockWise);
            }
            else if (line.EndNode == prevNode)
            {
                if (line.StartNode == cellStartNode)
                    return;
                if (clockWise)
                    GetLineAndMoveNext(line.StartNodeNextLineClockWise, cellLines, cellStartNode, line.StartNode, clockWise);
                else
                    GetLineAndMoveNext(line.StartNodeNextLineCounterClockWise, cellLines, cellStartNode, line.StartNode, clockWise);
            }
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

            Nodes = voronoiNodes.Values.ToList();
            ArrangeLinesInNodes();
        }

        public void CreateLines(IEnumerable<Edge> vornoiEdges, Polyline boarder)
        {
            List<double> parameters = new List<double>();
            List<VoronoiLine> lines = new List<VoronoiLine>();
            foreach (var edge in vornoiEdges)
            {
                Line l = new Line(StaticMethods.ToRhinoPoint(edge.Point1), StaticMethods.ToRhinoPoint(edge.Point2));
                if (l.Length > 0.0001)
                    lines.Add(new VoronoiLine(l));
            }
            /*
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
            */
            Lines = lines;
        }
    }
}
