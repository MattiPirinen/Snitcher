﻿using System;
using System.Collections.Generic;
using SnitchCommon;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SnitchGrasshopper.Component.HelpComponent
{
    public class VoronoiTest : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the VoronoiTest class.
        /// </summary>
        public VoronoiTest()
          : base("Snitch voronoi",
                "Snitch voronoi",
                "TestingVoroi",
                "Snitch",
                "Misc")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) 
        {
            pManager.AddPointParameter("Points", "Points", "Points", GH_ParamAccess.list);
            pManager.AddCurveParameter("BoarderPoints", "BoarderPoints", "BoarderPoints", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Line", "Line", "Line", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> pts = new List<Point3d>();
            if (!DA.GetDataList(0, pts))
                return;

            Curve c = null;
            if (!DA.GetData(1, ref c))
                return;

            if (!c.TryGetPolyline(out Polyline pl))
                return;

            PointCloud pcloud = new PointCloud(pts);
            BoundingBox bb =  pcloud.GetBoundingBox(false);

            List<Point2d> pts2d = new List<Point2d>();
            foreach (var item in pts)
            {
                pts2d.Add(new Point2d(item.X,item.Y));
            }
            List<Point2d> bpts2d = new List<Point2d>();

            List<Line> lines = StaticMethods.CreateVoronoi(pts2d, pl, bb);
            DA.SetDataList(0, lines);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("BC3CCFE1-4C09-4771-B92E-769D0974695D"); }
        }
    }
}