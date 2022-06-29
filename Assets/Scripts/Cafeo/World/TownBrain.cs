using System;
using UnityEngine;

namespace Cafeo.World
{
    public class TownBrain : MonoBehaviour
    {
        protected TownVessel vessel;
        public TownRegion Region => TownRegion.Instance;

        protected virtual void Start()
        {
            vessel = GetComponent<TownVessel>();
        }

        public virtual void DecideAction()
        {
            
        }
    }
}