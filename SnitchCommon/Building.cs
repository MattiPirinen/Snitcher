using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class Building
    {
        public Dictionary<Guid, Beam> Beams { get; } = new Dictionary<Guid, Beam>();
        public Dictionary<Guid, Column> Columns{ get; } = new Dictionary<Guid, Column>();
        public Dictionary<Guid, Slab> Slabs { get; } = new Dictionary<Guid, Slab>();
        public Dictionary<Guid, Wall > Walls { get; } = new Dictionary<Guid, Wall>();
    }
}
