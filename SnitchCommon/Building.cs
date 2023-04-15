using Rhino.UI.Controls.ThumbnailUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class Building : Building_base
    {
        //---------------------- CONSTRUCTORS ------------------------

        public Building()
        {
            AssignProperties();
        }

        //------------------------- FIELDS ---------------------------

        //----------------------- PROPERTIES -------------------------

        public List<Dictionary<Guid, BuildingMember_base>> BuildingObjectsList { get; private set; }
        public Dictionary<Guid, BuildingMember_base> Beams { get; private set; }
        public Dictionary<Guid, BuildingMember_base> Columns{ get; private set; }
        public Dictionary<Guid, BuildingMember_base> Slabs { get; private set; }
        public Dictionary<Guid, BuildingMember_base> Walls { get; private set; }

        private void InitiateObjectList()
        {
            this.BuildingObjectsList = new List<Dictionary<Guid, BuildingMember_base>>()
            {
                this.Beams,
                this.Columns,
                this.Walls,
                this.Slabs
            };
        }

        public override void Get_CO2(out double co2_total, out double co2_concrete, out double co2_steel)
        {
            co2_total = 0;
            co2_concrete = 0;
            co2_steel = 0;

            InitiateObjectList();

            foreach (Dictionary<Guid, BuildingMember_base> dict in this.BuildingObjectsList)
            {
                foreach (KeyValuePair<Guid, BuildingMember_base> kvp in dict)
                {
                    co2_concrete += kvp.Value.Get_CO2_concrete();
                    co2_steel += kvp.Value.Get_CO2_steel();
                }
            }

            co2_total = co2_concrete + co2_steel;
        }

        //------------------------ METHODS ---------------------------
        private void AssignProperties()
        {
            this.Beams = new Dictionary<Guid, BuildingMember_base>();
            this.Columns = new Dictionary<Guid, BuildingMember_base>();
            this.Slabs = new Dictionary<Guid, BuildingMember_base>();
            this.Walls= new Dictionary<Guid, BuildingMember_base>();
        }

    }
}
