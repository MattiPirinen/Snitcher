using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace SnitchGrasshopper
{
    public class SnitchGrasshopperInfo : GH_AssemblyInfo
    {
        public override string Name => "SnitchGrasshopper";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("034533c8-bf48-4425-aff9-d85867d787d6");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}