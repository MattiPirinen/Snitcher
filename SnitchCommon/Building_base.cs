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


        public decimal Volume_conc { get; set; }
        public decimal Volume_steel { get; set; }

        

        //------------------------ METHODS ---------------------------

        public abstract void Get_CO2(out double co2_total, out double co2_concrete, out double co2_steel);
        
    }
}
