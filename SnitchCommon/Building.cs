using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnitchCommon
{
    public class Building : Building_base
    {
        //---------------------- CONSTRUCTORS ------------------------

        public Building()
        {
            AssignProperties();
            this.Calculation = new Calculation(this);
        }
        public Building(List<BuildingMember_base> gh_inputObjs)
        {
            AssignProperties();
            this.Calculation = new Calculation(this, gh_inputObjs);
        }

        //----------------------- PROPERTIES -------------------------
        public Calculation Calculation { get; set; }

        public int FloorQty_total { get; set; }
        public double DistributedLoad_live { get; set; }

        public CO2Emission CO2_total { get; set; }
        public CO2Emission CO2_beams { get; set; }
        public CO2Emission CO2_columns { get; set; }
        public CO2Emission CO2_slabs { get; set; }
        public CO2Emission CO2_walls { get; set; }

        
        public Dictionary<Guid, BuildingMember_base> Beams { get; private set; }
        public Dictionary<Guid, BuildingMember_base> Columns{ get; private set; }
        public Dictionary<Guid, BuildingMember_base> Slabs { get; private set; }
        public Dictionary<Guid, BuildingMember_base> Walls { get; private set; }


        //------------------------ METHODS ---------------------------

        private void AssignProperties()
        {
            this.Beams = new Dictionary<Guid, BuildingMember_base>();
            this.Columns = new Dictionary<Guid, BuildingMember_base>();
            this.Slabs = new Dictionary<Guid, BuildingMember_base>();
            this.Walls= new Dictionary<Guid, BuildingMember_base>();

            this.DistributedLoad_live = 2000; // N/m2
        }

    }
}
