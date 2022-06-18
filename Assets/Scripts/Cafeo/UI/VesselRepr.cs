using System;
using Cafeo.World;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo.UI
{
    /// <summary>
    /// A UI representation of an agent
    /// </summary>
    public class VesselRepr : MonoBehaviour
    {
        public TownVessel vessel;

        public void Start()
        {
            Assert.IsNotNull(vessel);
            SetupMyself();
        }

        private void SetupMyself()
        {
            
        }
    }
}