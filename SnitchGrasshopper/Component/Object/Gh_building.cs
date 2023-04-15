using Grasshopper.Kernel;
using SnitchCommon;
using System;
using System.Collections.Generic;

namespace SnitchGrasshopper.Component.Object
{
    public class Gh_building : GH_Component
    {
        //---------------------- CONSTRUCTORS ------------------------

        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Gh_building()
          : base(
                "Snitch building",
                "Snitch building",
                "Assemble a Snitch building.",
                "Snitch",
                "Object")
        {

        }


        //----------------------- PROPERTIES -------------------------

        protected override System.Drawing.Bitmap Icon { get { return null; } }

        public override Guid ComponentGuid
        {
            get { return new Guid("467FFAF5-EFED-4279-84FA-BED43AE24169"); }
        }

        public List<BuildingMember_base> InputObjects { get; set; }
        public Building Building { get; set; }

        //------------------------ METHODS ---------------------------

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //#0
            pManager.AddGenericParameter("List of building objects", "Objects", "List of building objects", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //#0
            pManager.AddGenericParameter("Building", "Building", "Building", GH_ParamAccess.item);
        }

        private bool CollectInputData_buildingObjects(IGH_DataAccess DA)
        {
            List<BuildingMember_base> list_temp = new List<BuildingMember_base>();

            if (!DA.GetData(0, ref list_temp))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Failed to receive input for list of building objects");
                return false;
            }

            this.InputObjects = list_temp;

            return true;
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if(CollectInputData_buildingObjects(DA) ==false) { return; }


            this.Building = new Building(this.InputObjects);

            AssignOutputVariables(DA);
        }


        private void AssignOutputVariables(IGH_DataAccess DA)
        {
            DA.SetData(0, this.Building);
        }
    }
}