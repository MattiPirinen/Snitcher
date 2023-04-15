using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using SnitchCommon;
using SnitchGrasshopper.Properties;
using System;
using System.Collections.Generic;

namespace SnitchGrasshopper.Component.Object
{
    public class AssembleSnitchSlabComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public AssembleSnitchSlabComponent()
          : base(
                "Snitch slab",
                "Snitch slab",
                "Assemble a Snitch slab.",
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
            pManager.AddCurveParameter("Polyline", "P", "Polyline", GH_ParamAccess.item);
            pManager.AddNumberParameter("Concrete volume", "CV", "Concrete volume", GH_ParamAccess.item);
            pManager.AddNumberParameter("Steel mass", "SM", "Steel mass", GH_ParamAccess.item);
            pManager.AddTextParameter("Concrete class", "CC", "Concrete class", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "T", "Thickness", GH_ParamAccess.item);
            pManager.AddNumberParameter("Load", "L", "Load", GH_ParamAccess.item);

            pManager[6].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Snitch slab", "SS", "Snitch slab", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            Curve curve = null;
            double concreteVolume = double.NaN;
            double steelMass = double.NaN;
            string concreteClass = string.Empty;
            double thickness = double.NaN;
            double load = double.NaN;

            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetData(1, ref curve)) return;
            if (!DA.GetData(2, ref concreteVolume)) return;
            if (!DA.GetData(3, ref steelMass)) return;
            if (!DA.GetData(4, ref concreteClass)) return;
            if (!DA.GetData(5, ref thickness)) return;
            if (!DA.GetData(6, ref load)) return;

            curve.TryGetPolyline(out Polyline polyline);

            SnitchCommon.Slab slab = new SnitchCommon.Slab
            {
                Mesh = mesh,
                Boundary = polyline,
                Volume_concrete_m3 = concreteVolume,
                Mass_steel_m3 = steelMass,
                ConcreteClass = concreteClass,
                Load = load,                 
            };

            DA.SetData(0, slab);
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
        public override Guid ComponentGuid => new Guid("D88460DC-8039-4AC7-969A-FB79A3F59A54");
    }
}