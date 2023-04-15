using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class Building
    {
        //---------------------- CONSTRUCTORS ------------------------

        public Building()
        {
            AssignProperties();
        }

        //------------------------- FIELDS ---------------------------

        //----------------------- PROPERTIES -------------------------

        public Dictionary<Guid, Beam> Beams { get; private set; }
        public Dictionary<Guid, Column> Columns{ get; private set; }
        public Dictionary<Guid, Slab> Slabs { get; private set; }
        public Dictionary<Guid, Wall > Walls { get; private set; }


        //------------------------ METHODS ---------------------------
        private void AssignProperties()
        {
            this.Beams = new Dictionary<Guid, Beam>();
            this.Columns = new Dictionary<Guid, Column>();
            this.Slabs = new Dictionary<Guid, Slab>();
            this.Walls= new Dictionary<Guid, Wall>();
        }

    }
}
