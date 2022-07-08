using System;
using System.Collections.Generic;

namespace Cafeo.World
{
    public class RelationshipBuilder
    {
        public TownVessel parent;

        public RelationshipBuilder(TownVessel parent)
        {
            this.parent = parent;
        }

        private void CreateIfNotExists(TownVessel rhs)
        {
            if (rhs == parent) throw new Exception("Cannot create relationship with self");
            var rel = rhs.relationships;
            if (rel.ContainsKey(rhs))
                return;
            rel.Add(rhs, new HashSet<Relationship>());
            var myRels = parent.relationships;
            if (myRels.ContainsKey(rhs))
                return;
            myRels.Add(rhs, new HashSet<Relationship>());
        }

        public void AddSlave(TownVessel rhs)
        {
            CreateIfNotExists(rhs);
            var themToMe = rhs.relationships[parent];
            var meToThem = parent.relationships[rhs];
            themToMe.Add(Relationship.Slave);
            meToThem.Add(Relationship.Master);
        }
    }
}