using Newtonsoft.Json;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SnitchCommon
{
    public class Calculation
    {
        //---------------------- CONSTRUCTORS ------------------------
        public Calculation(Building building)
        {
            //Building = building;

            //DetectAndPopulateObjects(gh_inputObjs);
            //ProcessFloorInformation();

            InstantiateDataBAse();
        }
        public Calculation(Building building, List<BuildingMember_base> gh_inputObjs)
        {
            Building = building;

            DetectAndPopulateObjects(gh_inputObjs);
            ProcessFloorInformation();

            InstantiateDataBAse();
        }

        //----------------------- PROPERTIES -------------------------

        public Building DataBase { get; set; }
        public Building Building { get; set; }
        public List<Dictionary<Guid, BuildingMember_base>> BuildingObjectsList { get; private set; }


        //------------------------ METHODS ---------------------------

        private void InitiateObjectList()
        {
            this.BuildingObjectsList = new List<Dictionary<Guid, BuildingMember_base>>()
            {
                this.Building.Beams,
                this.Building.Columns,
                this.Building.Walls,
                this.Building.Slabs
            };
        }

        public void Calculate_CO2_and_score(AverageCo2Values averageCo2Values)
        {

            InitiateObjectList();

            foreach (Dictionary<Guid, BuildingMember_base> dict in this.BuildingObjectsList)
            {
                foreach (KeyValuePair<Guid, BuildingMember_base> kvp in dict)
                {
                    kvp.Value.CalculateProperties(averageCo2Values);

                    CollectCO2(kvp.Value);
                }
            }
        }

        private void CollectCO2(BuildingMember_base obj)
        {
            this.Building.CO2_total.CollectCO2(obj.CO2);

            if (obj is Beam)
            {
                this.Building.CO2_beams.CollectCO2(obj.CO2);
            }
            else if (obj is Column)
            {
                this.Building.CO2_columns.CollectCO2(obj.CO2);
            }
            else if (obj is Wall)
            {
                this.Building.CO2_walls.CollectCO2(obj.CO2);
            }
            else if (obj is Slab)
            {
                this.Building.CO2_slabs.CollectCO2(obj.CO2);
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
            if (item is Beam beam)
            {
                this.Building.Beams.Add(beam.Guid, beam);
            }
            else if (item is Column column)
            {
                this.Building.Columns.Add(column.Guid, column);
            }
            else if (item is Wall wall)
            {
                this.Building.Walls.Add(wall.Guid, wall);
            }
            else if (item is Slab slab)
            {
                this.Building.Slabs.Add(slab.Guid, slab);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        private void ProcessFloorInformation()
        {
            List<KeyValuePair<double, List<Column>>> list = CollectFloorColumns();

            this.Building.FloorQty_total = list.Count;

            SetFloorNumberAndLoadToColumns(list);
        }

        private List<KeyValuePair<double, List<Column>>> CollectFloorColumns()
        {
            Dictionary<double, List<Column>> minZCoordinates = new Dictionary<double, List<Column>>();

            foreach (Column column in this.Building.Columns.Values)
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

                    column.CalculateLoad(this.Building.FloorQty_total, this.Building.DistributedLoad_live);
                }


            }
        }

        private void InstantiateDataBAse()
        {
            string jsonString = ReadDataBase_generic("Database");
            List < BuildingMember_base > list = JsonConvert.DeserializeObject<List<BuildingMember_base>>(jsonString);
        }


        private string ReadDataBase_generic(string dbName)
        {
            string text = "";

            string path = GetResourcePath(dbName);

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(path))
            using (StreamReader reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }

        private string GetResourcePath(string keyword)
        {
            string path = "";

            List<string> resrourcePaths = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames().ToList();

            path = resrourcePaths.First(rp => rp.Contains(keyword));

            return path;
        }


    }
}
