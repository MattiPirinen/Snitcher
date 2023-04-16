using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class Slab: BuildingMember_base
    {
        //---------------------- CONSTRUCTORS ------------------------

        public Slab()
        {

        }

        //------------------------- FIELDS ---------------------------

        //----------------------- PROPERTIES -------------------------

        public Polyline Boundary { get; set; }
        public Dictionary<Point3d, Polyline> Voronois { get; } = new Dictionary<Point3d, Polyline>();
        public double Load { get; set; }

        //------------------------ METHODS ---------------------------
            
    }
}
