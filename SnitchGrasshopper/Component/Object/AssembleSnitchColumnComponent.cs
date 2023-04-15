using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using SnitchCommon;
using SnitchGrasshopper.Properties;
using System;
using System.Collections.Generic;

namespace SnitchGrasshopper.Component.Object
{
    public class AssembleSnitchColumnComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public AssembleSnitchColumnComponent()
          : base(
                "Snitch column",
                "Snitch column",
                "Assemble a Snitch column.",
                "Snitch", 
                "Object")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M ", "Mesh", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "P", "Points", GH_ParamAccess.list);
            pManager.AddNumberParameter("Length", "L", "Length", GH_ParamAccess.item);
            pManager.AddNumberParameter("Concrete volume", "CV", "Concrete volume", GH_ParamAccess.item);
            pManager.AddNumberParameter("Steel volume", "SV", "Steel volume", GH_ParamAccess.item);
            pManager.AddTextParameter("Concrete class", "CC", "Concrete class", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Snitch column", "SC", "Snitch column", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            List<Point3d> points = new List<Point3d>();
            double length = double.NaN;
            double concreteVolume = double.NaN;
            double steelVolume = double.NaN;
            string concreteClass = string.Empty;

            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetDataList(1, points)) return;
            if (!DA.GetData(2, ref length)) return;
            if (!DA.GetData(3, ref concreteVolume)) return;
            if (!DA.GetData(4, ref steelVolume)) return;
            if (!DA.GetData(5, ref concreteClass)) return;

            SnitchCommon.Column column = new SnitchCommon.Column
            {
                Mesh = mesh,
                Height = length,
                Volume_concrete_m3 = 0.0,
                Volume_steel_m3 = 0.0,
                ConcreteClass = "",
            };

            DA.SetData(0, column);
        }

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.DogSniffing_24x24;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("1A798EBA-8394-44E0-B653-4907BC79AF28");
    }
}