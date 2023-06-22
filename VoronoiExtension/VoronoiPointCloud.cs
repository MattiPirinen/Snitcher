using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoronoiExtension
{
    public class VoronoiPointCloud:PointCloud
    {
        public List<VoronoiNode> Nodes { get; } = new List<VoronoiNode>();


    }
}
