using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.World
{
    /// <summary>
    /// 
    /// </summary>
    public class TownOuterNode : TownNode
    {
        public List<TownInnerNode> interiorLocations;
        public TownRegion region;
        public override TownRegion Region => region;

        protected override void Awake()
        {
            base.Awake();
            interiorLocations = new List<TownInnerNode>();
        }

        protected override void OnRefresh()
        {
            base.OnRefresh();
            interiorLocations.Clear();
        }

        protected override void RefreshOnSee(GameObject trans)
        {
            base.RefreshOnSee(trans);
            if (trans.CompareTag("TownInner"))
            {
                var innerNode = trans.GetComponent<TownInnerNode>();
                interiorLocations.Add(innerNode);
                innerNode.Refresh();
                innerNode.parent = this;
            }
        }
    }
}