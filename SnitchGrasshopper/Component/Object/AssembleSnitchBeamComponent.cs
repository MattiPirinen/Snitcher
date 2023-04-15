using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using SnitchGrasshopper.Properties;
using System;
using System.Collections.Generic;

namespace SnitchGrasshopper.Component.Object
{
    public class AssembleSnitchBeamComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public AssembleSnitchBeamComponent()
          : base(
                "Snitch beam",
                "Snitch beam",
                "Assemble a Snitch beam.",
                "Snitch", 
                "Object")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Points", GH_ParamAccess.list);
            pManager.AddNumberParameter("Volume", "V", "Volume", GH_ParamAccess.item);
            pManager.AddMeshParameter("Mesh", "M ", "Mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Snitch beam", "SB", "Snitch beam", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            double volume = double.NaN;
            Mesh mesh = null;

            if (!DA.GetDataList(0, points)) return;
            if (!DA.GetData(1, ref volume)) return;
            if (!DA.GetData(2, ref mesh)) return;

            SnitchCommon.Beam beam = new SnitchCommon.Beam();
            //beam.Volume_concrete_m3 = volume;


            DA.SetData(0, beam);
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
        public override Guid ComponentGuid => new Guid("f3f05277-b36c-41b3-ab07-65e86ee5cf64");
    }
}