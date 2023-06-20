using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class VoronoiNode
    {
        public List<VoronoiLine> VoronoiLines { get; private set; } = new List<VoronoiLine>();

        public Point3d Location { get; set; }

        public void ArrangeLines()
        {
            List<(double angle, VoronoiLine line)> angleLines = new List<(double angle, VoronoiLine line)>();
            if (VoronoiLines.Count == 0) return;
            Vector3d firstDir = VoronoiLines[0].Line.Direction;
            if (VoronoiLines[0].StartNode != this) firstDir.Reverse();
            for (int i = 1; i < VoronoiLines.Count; i++)
            {
                Vector3d nextDir = VoronoiLines[i].Line.Direction;
                if (VoronoiLines[i].StartNode != this) nextDir.Reverse();
                angleLines.Add((Vector3d.VectorAngle(firstDir, nextDir, Plane.WorldXY), VoronoiLines[i]));
            }
            angleLines.Sort((x,y)=>x.angle.CompareTo(y.angle));
            List<VoronoiLine> voronoiLines = new List<VoronoiLine>();
            voronoiLines.Add(VoronoiLines[0]);
            for (int i = 0; i < angleLines.Count; i++)
            {
                voronoiLines.Add(angleLines[i].line);
            }
            for (int i = 0; i < voronoiLines.Count; i++)
            {
                if (voronoiLines[i].StartNode == this)
                {
                    voronoiLines[i].StartNodeNextLineCounterClockWise = i == 0 ? voronoiLines.Last() : voronoiLines[i - 1];
                    voronoiLines[i].StartNodeNextLineClockWise = i == voronoiLines.Count - 1 ? voronoiLines[0] : voronoiLines[i + 1];
                }
                else
                {
                    voronoiLines[i].EndNodeNextLineCounterClockWise = i == 0 ? voronoiLines.Last() : voronoiLines[i - 1];
                    voronoiLines[i].EndNodeNextLineClockWise = i == voronoiLines.Count - 1 ? voronoiLines[0] : voronoiLines[i + 1];
                }

            }

            VoronoiLines= voronoiLines;
        }


    }
}
