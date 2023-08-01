using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public abstract class MemberBase: BuildingMember_base
    {
        public Line CenterLine { get; set; }

    }
}
