using System;
using UnityEngine;

namespace Cafeo.World
{
    public class TownBrain : MonoBehaviour
    {
        protected TownVessel vessel;

        private void Start()
        {
            vessel = GetComponent<TownVessel>();
        }

        public virtual void DecideAction()
        {
            
        }
    }
}