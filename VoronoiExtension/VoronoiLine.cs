using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoronoiExtension
{
    public class VoronoiLine
    {
        public VoronoiLine(Line line)
        {
            Line = line;
        }

        public Line Line { get; set; }

        public VoronoiNode StartNode { get; set; }
        public VoronoiNode EndNode { get; set; }


        public VoronoiLine StartNodeNextLineClockWise { get; set; }
        public VoronoiLine StartNodeNextLineCounterClockWise { get; set; }

        public VoronoiLine EndNodeNextLineClockWise { get; set; }
        public VoronoiLine EndNodeNextLineCounterClockWise { get; set; }

        public bool IsEdgeLine { get; set; } = false;

        public bool UsedClockWise { get; set; } = false;
        public bool UsedCounterClockWise { get; set; } = false;
    }
}
