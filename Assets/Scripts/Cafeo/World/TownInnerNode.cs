using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.World
{
    /// <summary>
    /// An interior node of the town, connecting to other interior nodes
    /// </summary>
    public class TownInnerNode : TownNode
    {
        public List<TownInteriorNode> rooms;
        public TownOuterNode parent;
        // public Sprite bgSprite;

        protected override void OnRefresh()
        {
            base.OnRefresh();
            rooms.Clear();
        }

        protected override void RefreshOnSee(GameObject trans)
        {
            base.RefreshOnSee(trans);
            if (trans.CompareTag("TownInterior"))
            {
                var interiorNode = trans.GetComponent<TownInteriorNode>();
                rooms.Add(interiorNode);
                interiorNode.parent = this;
                interiorNode.Refresh();
            }
        }
    }
}