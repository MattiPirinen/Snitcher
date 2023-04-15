using System;
using System.Collections.Generic;

namespace SnitchCommon
{
    public class Building : Building_base
    {
        //---------------------- CONSTRUCTORS ------------------------

        public Building()
        {
            AssignProperties();
        }

        public Building(List<BuildingMember_base> gh_inputObjs)
        {
            AssignProperties();
            DetectAndPopulateObjects(gh_inputObjs);
        }

        //----------------------- PROPERTIES -------------------------
        public int NoOfFloors { get; set; }
        public List<Dictionary<Guid, BuildingMember_base>> BuildingObjectsList { get; private set; }
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
        }

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

        public void Get_CO2(out double co2_total, out double co2_concrete, out double co2_steel)
        {
            co2_total = 0;
            co2_concrete = 0;
            co2_steel = 0;

            InitiateObjectList();

            foreach (Dictionary<Guid, BuildingMember_base> dict in this.BuildingObjectsList)
            {
                foreach (KeyValuePair<Guid, BuildingMember_base> kvp in dict)
                {
                    kvp.Value.CalculateProperties();

                    co2_concrete += (double)kvp.Value.CO2_concrete;
                    co2_steel += (double)kvp.Value.CO2_steel;
                }
            }

            co2_total = co2_concrete + co2_steel;
        }

        private void DetectAndPopulateObjects(List<BuildingMember_base> gh_inputObjs)
        {
            foreach (BuildingMember_base item in gh_inputObjs)
            {
                DetectAndPopulateObject(item);
            }
        }

        private void DetectAndPopulateObject(BuildingMember_base item)
        {
            if(item is Beam beam) 
            { 
                this.Beams.Add(beam.Guid, beam); 
            }
            else if(item is Column column)
            {
                this.Columns.Add(column.Guid, column);
            }
            else if(item is Wall wall)
            {
                this.Walls.Add(wall.Guid, wall);
            }
            else if(item is Slab slab)
            {
                this.Slabs.Add(slab.Guid, slab);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

    }
}
