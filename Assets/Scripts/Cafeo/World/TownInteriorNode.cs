using System.Collections.Generic;
using System.Linq;

namespace Cafeo.World
{
    public class TownInteriorNode : TownNode
    {
        public TownInnerNode parent;
        // public Sprite bgSprite;

        public IEnumerable<TownInteriorNode> Neighbors
        {
            get { return parent.rooms.Where(node => node != this); }
        }
    }
}