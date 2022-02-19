using System;
using BehaviorDesigner.Runtime;
using Cafeo.Castable;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo.Aimer
{
    public class RangedAimer : GenericAimer<RangedItem>
    {
        public override RangedItem Item { get; set; }

        public override void ManualAim()
        {
            base.ManualAim();
        }
    }
}