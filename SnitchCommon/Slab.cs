﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class Slab: BuildingMember_base
    {
        //---------------------- CONSTRUCTORS ------------------------

        public Slab()
        {

        }

        //------------------------- FIELDS ---------------------------

        //----------------------- PROPERTIES -------------------------

        public Polyline Boundary { get; set; }
        public Dictionary<Point3d, Polyline> Voronois { get; } = new Dictionary<Point3d, Polyline>();
        public double Load { get; set; }

        //------------------------ METHODS ---------------------------
        override protected void Set_CO2_concrete()
        {
            double area = AreaMassProperties.Compute(this.Boundary.ToNurbsCurve()).Area * Math.Pow(10, -6);
            base.Set_CO2_concrete();
            CO2.Concrete = CO2.Concrete / area;
        }
        protected override void Set_CO2_steel()
        {
            double area = AreaMassProperties.Compute(this.Boundary.ToNurbsCurve()).Area * Math.Pow(10, -6);
            base.Set_CO2_steel();
            CO2.Steel = CO2.Steel / area;
        }
    }
}
