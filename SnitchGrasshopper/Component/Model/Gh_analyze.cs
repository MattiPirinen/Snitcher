using Grasshopper.Kernel;
using Newtonsoft.Json;
using SnitchCommon;
using SnitchGrasshopper.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SnitchGrasshopper.Component.Model
{
    public class Gh_analyze : GH_Component
    {
        //---------------------- CONSTRUCTORS ------------------------

        public Gh_analyze()
          : base(
                "Analyze Snitch building",
                "Analyze Snitch building",
                "Analyze a Snitch building.",
                "Snitch",
                "Model")
        {

        }

        //----------------------- PROPERTIES -------------------------

        protected override System.Drawing.Bitmap Icon => Resources.DogSniffing_24x24;

        public override Guid ComponentGuid
        {
            get { return new Guid("3FE0718F-C4F6-4612-A9B9-06DD31934B76"); }
        }

        public Building Building { get; set; }
        public Building DataBase { get; set; }

        //------------------------ METHODS ---------------------------

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Building", "Building", "building object", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Building", "Building", "Building", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            CollectInputData(DA);
            List<Building> dataBaseBuildings = GetDatabaseBuildings();


            this.Building.Calculate_CO2_and_score(dataBaseBuildings);

            AssignOutputVariables(DA);
        }

        private List<Building> GetDatabaseBuildings()
        {
            List<Building> databaseBuildings = new List<Building>();

            string databaseDirectory =
                $"{Directory.GetParent(OnPingDocument().FilePath).FullName}{Path.DirectorySeparatorChar}";

            foreach (string filePath in Directory.GetFiles(databaseDirectory))
            {
                if (filePath.Contains(".json"))
                {
                    string json = string.Empty;

                    using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
                    {
                        json = streamReader.ReadToEnd();
                    }

                    Building databse = JsonConvert.DeserializeObject<Building>(json);
                    databaseBuildings.Add(databse);
                }
            }

            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, $"Using {databaseBuildings.Count} database buildings");

            return databaseBuildings;
        }


        private bool CollectInputData(IGH_DataAccess DA)
        {
            if (CollectInputData_building(DA) == false) { return false; }

            return true;
        }

        private bool CollectInputData_building(IGH_DataAccess DA)
        {
            Building building_temp = new Building();


            if (!DA.GetData(0, ref building_temp))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Failed to receive input for building");
                return false;
            }

            this.Building = building_temp;

            return true;
        }


        private void AssignOutputVariables(IGH_DataAccess DA)
        {
            DA.SetData(0, Building);
        }
    }
}