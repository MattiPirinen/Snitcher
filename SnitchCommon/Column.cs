using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class Column: BuildingMember_base
    {
        //---------------------- CONSTRUCTORS ------------------------

        //------------------------- FIELDS ---------------------------

        //----------------------- PROPERTIES -------------------------
        
        public double Height { get; set; }
        public double LoadBearingArea { get; set; }
        public double Load { get; set; }
        public Line CenterLine { get; set; }

        //------------------------ METHODS ---------------------------

        public void CalculateLoad(int floorQty_tot, double floorLoad)
        {
            int topFloorsQty = floorQty_tot - this.FloorNo + 1 /*roof*/;

            this.Load = topFloorsQty * this.LoadBearingArea * floorLoad;
        }
    }
}
