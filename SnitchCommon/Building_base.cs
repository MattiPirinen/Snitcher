using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public abstract class Building_base
    {
        //---------------------- CONSTRUCTORS ------------------------
        public Building_base()
        {

        }

        //------------------------- FIELDS ---------------------------

        //----------------------- PROPERTIES -------------------------


        public decimal Volume_concrete_m3 { get; set; }
        public decimal Volume_steel_m3 { get; set; }

        public decimal Weight_concrete_N { get; set; }
        public decimal Weight_steel_N { get; set; }

        public decimal CO2_total { get; set; }
        public decimal CO2_concrete { get; set; }
        public decimal CO2_steel { get; set; }

        //------------------------ METHODS ---------------------------

    }
}
