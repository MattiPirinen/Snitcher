using Newtonsoft.Json;
using Rhino;
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
        }

        public Building(List<BuildingMember_base> gh_inputObjs)
        {
            AssignProperties();
            DetectAndPopulateObjects(gh_inputObjs);
            ProcessFloorInformation();
        }

        //----------------------- PROPERTIES -------------------------
        public int FloorQty_total { get; set; }
        public double DistributedLoad_live { get; set; }
        public double CO2_total { get; private set; }
        public double CO2_concrete { get; private set; }
        public double CO2_steel { get; private set; }

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

            this.DistributedLoad_live = 2000; // N/m2
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

        public void Calculate_CO2_and_score(AverageCo2Values averageCo2Values)
        {
            this.CO2_total = 0;
            this.CO2_concrete = 0;
            this.CO2_steel = 0;

            InitiateObjectList();

            foreach (Dictionary<Guid, BuildingMember_base> dict in this.BuildingObjectsList)
            {
                foreach (KeyValuePair<Guid, BuildingMember_base> kvp in dict)
                {
                    kvp.Value.CalculateProperties(averageCo2Values);

                    this.CO2_concrete += (double)kvp.Value.CO2_concrete;
                    this.CO2_steel += (double)kvp.Value.CO2_steel;
                }
            }

            this.CO2_total = this.CO2_concrete + this.CO2_steel;
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

        
        private void ProcessFloorInformation()
        {
            List<KeyValuePair<double, List<Column>>> list = CollectFloorColumns();

            this.FloorQty_total = list.Count;

            SetFloorNumberAndLoadToColumns(list);
        }

        private List<KeyValuePair<double, List<Column>>> CollectFloorColumns()
        {
            Dictionary<double, List<Column>> minZCoordinates = new Dictionary<double, List<Column>>();

            foreach (Column column in this.Columns.Values)
            {
                BoundingBox bb = BoundingBox.Empty;

                bb = column.CenterLine.ToNurbsCurve().GetBoundingBox(true);

                double minZ = Math.Round(bb.Min.Z, 1);

                if (minZCoordinates.ContainsKey(minZ))
                    minZCoordinates[minZ].Add(column);
                else
                    minZCoordinates.Add(minZ, new List<Column>() { column });
            }

            List<KeyValuePair<double, List<Column>>> list = minZCoordinates.ToList();

            return list.OrderBy(l => l.Key).ToList();
        }

        private void SetFloorNumberAndLoadToColumns(List<KeyValuePair<double, List<Column>>> list)
        {
            for (int i = 1; i <= list.Count; i++)
            {
                KeyValuePair<double, List<Column>> kvp = list[i];

                foreach (Column column in kvp.Value)
                {
                    column.FloorNo = i;

                    column.CalculateLoad(this.FloorQty_total, this.DistributedLoad_live);
                }


            }
        }

    }
}
