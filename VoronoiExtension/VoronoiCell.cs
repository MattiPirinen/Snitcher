using Rhino.Geometry;

namespace VoronoiExtension
{
    public class VoronoiCell
    {

        public Polyline Perimeter { get; set; }
        public VoronoiNode Node { get; set; }
    }
}