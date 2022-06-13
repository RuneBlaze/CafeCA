using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.World
{
    /// <summary>
    /// Parent class for all "locations" in the physical world
    /// </summary>
    public class TownNode : MonoBehaviour
    {
        public List<TownAgent> agents;
        public virtual TownRegion Region { get; }
        public string displayName;
        public Sprite bgImage;

        protected virtual void Awake()
        {
            agents = new List<TownAgent>();
            gameObject.tag = "TownExterior";
        }

        /// <summary>
        /// syncs internal pointers to those reflected in the world hierarchy
        /// </summary>
        public virtual void Refresh()
        {
            OnRefresh();
            var n = transform.childCount;
            for (int i = 0; i < n; i++)
            {
                var child = transform.GetChild(i);
                RefreshOnSee(child.gameObject);
            }
        }

        protected virtual void OnRefresh()
        {
            agents.Clear();
        }

        protected virtual void RefreshOnSee(GameObject trans)
        {
            if (trans.CompareTag("TownAgent"))
            {
                agents.Add(trans.GetComponent<TownAgent>());
            }
        }
    }
}