using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.World
{
    /// <summary>
    ///     Parent class for all "locations" in the physical world
    /// </summary>
    public class TownNode : MonoBehaviour
    {
        public List<TownVessel> vessels;
        public Dictionary<TownVessel, int> index;
        public string displayName;
        public Sprite bgImage;
        public virtual TownRegion Region { get; }

        protected virtual void Awake()
        {
            vessels = new List<TownVessel>();
            index = new Dictionary<TownVessel, int>();
            gameObject.tag = "TownExterior";
        }

        /// <summary>
        ///     syncs internal pointers to those reflected in the world hierarchy
        /// </summary>
        public virtual void Refresh()
        {
            OnRefresh();
            var n = transform.childCount;
            for (var i = 0; i < n; i++)
            {
                var child = transform.GetChild(i);
                RefreshOnSee(child.gameObject);
            }
        }

        protected virtual void OnRefresh()
        {
            vessels.Clear();
        }

        protected virtual void RefreshOnSee(GameObject trans)
        {
            if (trans.CompareTag("TownAgent"))
            {
                var vessel = trans.GetComponent<TownVessel>();
                AddVessel(vessel);
            }
        }
        
        /// <summary>
        /// Adds a vessel, but does NOT take care of object hierarchy
        /// </summary>
        /// <param name="vessel"></param>
        public void AddVessel(TownVessel vessel)
        {
            int i = vessels.Count;
            vessels.Add(vessel);
            index[vessel] = i;
            vessel.location = this;
        }

        /// <summary>
        /// Removes a vessel, but does NOT take care of object hierarchy
        /// </summary>
        /// <param name="vessel"></param>
        public void RemoveVessel(TownVessel vessel)
        {
            var v = vessels[^1];
            vessels.RemoveAt(vessels.Count - 1);
            index.Remove(vessel);
            vessels[index[vessel]] = v;
        }

        /// <summary>
        /// Move a vessel from one place to another, taking care of maintaining invariants.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="target"></param>
        public void Transfer(TownVessel payload, TownNode target)
        {
            RemoveVessel(payload);
            payload.gameObject.transform.parent = target.transform;
            target.AddVessel(payload);
        }
    }
}