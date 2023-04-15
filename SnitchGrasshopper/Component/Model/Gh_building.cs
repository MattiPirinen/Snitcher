using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SnitchCommon;
using System;
using System.Collections.Generic;

namespace SnitchGrasshopper.Component.Model
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

        public GH_Structure<GH_Curve> Voronois { get; set; }
        public Building Building { get; set; }

        //------------------------ METHODS ---------------------------

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            //#0
            pManager.AddGenericParameter("List of building objects", "Objects", "List of building objects", GH_ParamAccess.list);
            pManager.AddCurveParameter("Voronois", "Voronois", "Voronois", GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            //#0
            pManager.AddGenericParameter("Building", "Building", "Building", GH_ParamAccess.item);
        }

        private bool CollectInputData_buildingObjects(IGH_DataAccess DA)
        {
            List<BuildingMember_base> list_temp = new List<BuildingMember_base>();
            GH_Structure<GH_Curve> curves = new GH_Structure<GH_Curve>();
            if (!DA.GetDataList(0, list_temp))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Failed to receive input for list of building objects");
                return false;
            }
            if (!DA.GetDataTree(1, out curves))
                return false;
            
            


            InputObjects = list_temp;
            Voronois = curves;
            return true;
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (CollectInputData_buildingObjects(DA) == false) { return; }
            Building = new Building(InputObjects);
            List<PointCloud> pClouds = new List<PointCloud>();
            List<double> floorCoordsZ = new List<double>();
            


            foreach (var item in Voronois.Branches)
            {
                item[0].Value.TryGetPolyline(out var polyline);
                floorCoordsZ.Add(polyline[0].Z);
                PointCloud pl = new PointCloud();
                foreach (var item2 in item)
                {
                    pl.Add(AreaMassProperties.Compute(item2.Value).Centroid);
                }
                pClouds.Add(pl);
            }
            foreach (var column in Building.Columns.Values)
            {
                Column col = (Column)column;
                int i = 0;
                foreach (var item in floorCoordsZ)
                {
                    if (Math.Abs(item - col.CenterLine.To.Z) < 1)
                        break;
                    i++;
                }
                //var voronois = Voronois.get_Branch(i);
                //int index = pClouds[i].ClosestPoint(col.CenterLine.To);
                //col.LoadBearingArea = AreaMassProperties.Compute(((GH_Curve)voronois[index]).Value).Area;
            }



            AssignOutputVariables(DA);
        }


        private void AssignOutputVariables(IGH_DataAccess DA)
        {
            DA.SetData(0, Building);
        }
    }
}