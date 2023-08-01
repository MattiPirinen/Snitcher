using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class Column: MemberBase
    {
        //---------------------- CONSTRUCTORS ------------------------

        //------------------------- FIELDS ---------------------------

        //----------------------- PROPERTIES -------------------------
        
        public double Height { get; set; }
        public double LoadBearingArea { get; set; }
        public double NormalForce { get; set; }

        //------------------------ METHODS ---------------------------

        public void CalculateLoad(int floorQty_tot, double floorLoad)
        {
            int topFloorsQty = floorQty_tot - this.FloorNo + 1 /*roof*/;

            this.NormalForce = topFloorsQty * this.LoadBearingArea * floorLoad;
        }
    }
}
