using Grasshopper.Kernel;
using Newtonsoft.Json;
using SnitchCommon;
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
                "Snitch building",
                "Snitch building",
                "Assemble a Snitch building.",
                "Snitch",
                "Object")
        {

        }

        //----------------------- PROPERTIES -------------------------

        protected override System.Drawing.Bitmap Icon 
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("3FE0718F-C4F6-4612-A9B9-06DD31934B76"); }
        }

        public Building Building { get; set; }
        public Building DataBase { get; set; }

        //------------------------ METHODS ---------------------------

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("DataBase", "DataBase", "DataBase object", GH_ParamAccess.item);
            pManager.AddGenericParameter("Building", "Building", "building object", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Building", "Building", "Building", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            CollectInputData(DA);

            this.Building.Calculate_CO2_and_score(this.DataBase);

            AssignOutputVariables(DA);
        }


        private bool CollectInputData(IGH_DataAccess DA)
        {
            if (CollectInputData_dataBase(DA) == false) { return false; }

            if (CollectInputData_building(DA) == false) { return false; }

            return true;
        }


        private bool CollectInputData_dataBase(IGH_DataAccess DA)
        {
            Building dataBase_temp = new Building();


            if (!DA.GetData(0, ref dataBase_temp))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Failed to receive input for databse");
                return false;
            }

            this.DataBase = dataBase_temp;

            return true;
        }
        

        private bool CollectInputData_building(IGH_DataAccess DA)
        {
            Building building_temp = new Building();


            if (!DA.GetData(1, ref building_temp))
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