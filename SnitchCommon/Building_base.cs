using Newtonsoft.Json;
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
        [JsonConstructor]
        public Building_base(bool dummy = false)
        {
        }
        public Building_base()
        {
            AssignProperties();
        }

        //------------------------- FIELDS ---------------------------

        //----------------------- PROPERTIES -------------------------

        public Guid Guid { get; set; }

        public double Volume_concrete_m3 { get; set; }
        public double Mass_steel_m3 { get; set; }

        public double Weight_concrete_N { get; set; }
        public double Weight_steel_N { get; set; }

        public CO2Emission CO2 { get; set; } = new CO2Emission();
        public double Score { get; set; }


        //------------------------ METHODS ---------------------------

        private void AssignProperties()
        {
            this.Guid = Guid.NewGuid();
        }

    }
}
