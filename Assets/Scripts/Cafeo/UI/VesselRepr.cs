using System;
using Cafeo.World;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Cafeo.UI
{
    /// <summary>
    /// A UI representation of an agent
    /// </summary>
    public class VesselRepr : MonoBehaviour
    {
        public TownVessel vessel;
        private LayoutElement layoutElement;
        private TooltipTrigger tooltipTrigger;

        public void Start()
        {
            Assert.IsNotNull(vessel);
            layoutElement = GetComponent<LayoutElement>();
            tooltipTrigger = GetComponent<TooltipTrigger>();
            SetupMyself();
        }

        private void SetupMyself()
        {
            tooltipTrigger.text = vessel.soul.DisplayName;
            var size = vessel.soul.HeightScore * 100;
            layoutElement.preferredWidth = size;
            layoutElement.preferredHeight = size;
        }
    }
}