using Newtonsoft.Json;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SnitchCommon
{
    public class Building : Building_base
    {
        //---------------------- CONSTRUCTORS ------------------------

        public Building()
        {
            
        }

        public Building(List<BuildingMember_base> gh_inputObjs)
        {
            AssignProperties();
            DetectAndPopulateObjects(gh_inputObjs);
            ProcessFloorInformation();
        }

        //----------------------- PROPERTIES -------------------------

        [JsonIgnore]
        public List<Dictionary<Guid, BuildingMember_base>> BuildingObjectsList { get; private set; }


        public int FloorQty_total { get; set; }
        public double DistributedLoad_live { get; set; } 

        public CO2Emission CO2_total { get; set; }
        public CO2Emission CO2_beams { get; set; }
        public CO2Emission CO2_columns { get; set; }
        public CO2Emission CO2_slabs { get; set; }
        public CO2Emission CO2_walls { get; set; }

        
        public Dictionary<Guid, Beam> Beams { get; set; } = new Dictionary<Guid, Beam>();
        public Dictionary<Guid, Column> Columns { get; set; } = new Dictionary<Guid, Column>();
        public Dictionary<Guid, Slab> Slabs { get; set; } = new Dictionary<Guid, Slab>();
        public Dictionary<Guid, Wall> Walls { get; set; } = new Dictionary<Guid, Wall>();


        //------------------------ METHODS ---------------------------

        private void AssignProperties()
        {
            this.Beams = new Dictionary<Guid, Beam>();
            this.Columns = new Dictionary<Guid, Column>();
            this.Slabs = new Dictionary<Guid, Slab>();
            this.Walls= new Dictionary<Guid, Wall>();

            this.DistributedLoad_live = 4000; // N/m2
        }

        //private void InitiateObjectList()
        //{
        //    BuildingObjectsList = new List<Dictionary<Guid, BuildingMember_base>>()
        //    {
        //        this.Beams,
        //        this.Columns,
        //        this.Walls,
        //        this.Slabs
        //    };
        //}

        public void Calculate_CO2_and_score(List<Building> dataBaseBuildings)
        {
            foreach (KeyValuePair<Guid, Column> kvp in Columns)
            {
                CO2Emission co2_ref = Get_co2_fromClosest_column(dataBaseBuildings.SelectMany(db => db.Columns.Values).ToList(), kvp.Value);
                kvp.Value.CalculateProperties(co2_ref);

                CollectCO2(kvp.Value);
            }

            foreach (KeyValuePair<Guid, Slab> kvp in Slabs)
            {
                CO2Emission co2_ref = Get_co2_fromClosest_slab();
                kvp.Value.CalculateProperties(co2_ref);

                CollectCO2(kvp.Value);
            }
        }

        public void Calculate_CO2()
        {
            CO2_columns = new CO2Emission();
            CO2_slabs = new CO2Emission();
            CO2_total = new CO2Emission();

            foreach (KeyValuePair<Guid, Column> kvp in Columns)
            {
                kvp.Value.CalculateCO2();
                CO2_columns.Steel += kvp.Value.CO2.Steel;
                CO2_columns.Concrete += kvp.Value.CO2.Concrete;
                CO2_columns.Total += kvp.Value.CO2.Steel + kvp.Value.CO2.Concrete;
            }

            foreach (KeyValuePair<Guid, Slab> kvp in Slabs)
            {
                double area = AreaMassProperties.Compute(kvp.Value.Boundary.ToNurbsCurve()).Area*Math.Pow(10,-6);
                kvp.Value.CalculateCO2();
                CO2_slabs.Steel += kvp.Value.CO2.Steel* area;
                CO2_slabs.Concrete += kvp.Value.CO2.Concrete* area;
                CO2_slabs.Total += kvp.Value.CO2.Steel* area + kvp.Value.CO2.Concrete* area;
            }

            CO2_total.Concrete = CO2_columns.Concrete + CO2_slabs.Concrete;
            CO2_total.Steel = CO2_columns.Steel + CO2_slabs.Steel;
            CO2_total.Total = CO2_columns.Total + CO2_slabs.Total;
        }

        private CO2Emission Get_co2_fromClosest_column(List<Column> list_in_base, Column column_curr)
        {
            List<Column> list_in_columns = new List<Column>();

            list_in_base.ForEach(x => list_in_columns.Add(x));

            List<(double, Column)> list_diff = new List<(double, Column)>();

            foreach (Column col_in in list_in_columns)
            {
                double diff = Math.Abs(col_in.NormalForce - column_curr.NormalForce);
                list_diff.Add((diff, col_in));
            }

            List<(double, Column)> list_diff_sorted = list_diff.OrderBy(x => x.Item1).ToList();

            return list_diff_sorted[0].Item2.CO2;
        }
        
        private CO2Emission Get_co2_fromClosest_slab()
        {
            return new CO2Emission()
            {
                Total = 76.0
            };
        }

        private void CollectCO2(BuildingMember_base obj)
        {
            this.CO2_total.CollectCO2(obj.CO2);

            if (obj is Beam)
            {
                this.CO2_beams.CollectCO2(obj.CO2);
            }
            else if (obj is Column)
            {
                this.CO2_columns.CollectCO2(obj.CO2);
            }
            else if (obj is Wall)
            {
                this.CO2_walls.CollectCO2(obj.CO2);
            }
            else if (obj is Slab)
            {
                this.CO2_slabs.CollectCO2(obj.CO2);
            }
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
            if (item is null)
                return;
            else if (item is Beam beam)
            {
                this.Beams.Add(beam.Guid, beam);
            }
            else if (item is Column column)
            {
                this.Columns.Add(column.Guid, column);
            }
            else if (item is Wall wall)
            {
                this.Walls.Add(wall.Guid, wall);
            }
            else if (item is Slab slab)
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

            SetFloorNumberToColumns(list);
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

        private void SetFloorNumberToColumns(List<KeyValuePair<double, List<Column>>> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                KeyValuePair<double, List<Column>> kvp = list[i];

                foreach (Column column in kvp.Value)
                {
                    column.FloorNo = i + 1;

                    
                }


            }
        }

        public void SetColumnLoads()
        {
            foreach (var column in Columns.Values)
            {
                Column col = (Column)column;
                col.CalculateLoad(this.FloorQty_total, this.DistributedLoad_live);
            }
        }

        public void CalculateBeamLoadBearingWidths()
        {
            Dictionary<int, List<Beam>> floorBeams = new Dictionary<int, List<Beam>>();
            foreach (var beam in Beams.Values)
            {
                if (floorBeams.ContainsKey(beam.FloorNo))
                    floorBeams[beam.FloorNo].Add(beam);
                else
                    floorBeams.Add(beam.FloorNo, new List<Beam>() { beam});
            }

            foreach (var beamsByFloor in floorBeams)
            {
                CalculateBeamLoadBearingWidthsByFloor(beamsByFloor.Value);
            }

            
        }

        private void CalculateBeamLoadBearingWidthsByFloor(List<Beam> beams)
        {
            List<Line> centerLines = beams.Select(b=>b.CenterLine).ToList();
            foreach (var beam in beams)
            {
                Line centerLine = beam.CenterLine;

                Point3d centerPt = centerLine.From + centerLine.To / 2;
                Vector3d v1 = Vector3d.CrossProduct(centerLine.Direction, Vector3d.ZAxis);
                Vector3d v2 = Vector3d.CrossProduct(centerLine.Direction, -Vector3d.ZAxis);

                Line l1 = new Line(centerPt, v1 * 1000000);
                double minDistance1 = CalculateMinDistance(centerLines, centerPt, l1);

                Line l2 = new Line(centerPt, v2 * 1000000);
                double minDistance2 = CalculateMinDistance(centerLines, centerPt, l2);

                beam.LoadBearingWidth  = minDistance1/2 + minDistance2/2;
            }
        }

        private static double CalculateMinDistance(List<Line> centerLines, Point3d centerPt, Line l1)
        {
            double minDistance = double.MaxValue;
            foreach (var line in centerLines)
            {
                if (!Intersection.LineLine(l1, line, out double a, out double b))
                    continue;
                if (a <= 0 || a > 1 || b < 0 || b > 1)
                    continue;
                Point3d intersectionPt = l1.PointAt(a);
                double distance = (centerPt - intersectionPt).Length;
                if (minDistance > distance)
                    minDistance = distance;
            }
            if (minDistance == double.MaxValue)
                minDistance = 0;
            return minDistance;
        }
    }
}
