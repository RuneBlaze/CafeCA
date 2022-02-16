using System;
using Cafeo.Castable;
using Drawing;
using UnityEngine;

namespace Cafeo.Aimer
{
    public class TossAimer : GenericAimer<TossItem>
    {
        public override TossItem Item { get; set; }

        public void SetMaxDistance(float value)
        {
            BehaviorTree.SetVariableValue("MaxDistance", value);
        }

        public void Update()
        {
            if (Item == null || hidden)
            {
                return;
            }
            var targetObject = BehaviorTree.GetVariable("TargetObject").GetValue() as GameObject;
            if (targetObject != null)
            {
                var draw = Draw.ingame;
                draw.Arrow(transform.position, targetObject.transform.position);
            }
        }
    }
}