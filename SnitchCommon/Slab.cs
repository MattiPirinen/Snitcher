﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class Slab: BuildingObjects
    {
        //---------------------- CONSTRUCTORS ------------------------

        public Slab()
        {

        }

        //------------------------- FIELDS ---------------------------

        //----------------------- PROPERTIES -------------------------

        public Polyline Boundary { get; set; }

        //------------------------ METHODS ---------------------------
    }
}
