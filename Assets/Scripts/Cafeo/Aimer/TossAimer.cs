using System.Collections.Generic;
using Cafeo.Castable;
using Drawing;
using UnityEngine;

namespace Cafeo.Aimer
{
    public class TossAimer : GenericAimer<TossItem>
    {
        private int targetMask;
        public override TossItem Item { get; set; }

        public override void Update()
        {
            base.Update();
            if (Item == null || hidden) return;
            var targetObject = BehaviorTree.GetVariable("TargetObject").GetValue() as GameObject;
            if (targetObject != null)
            {
                var draw = Draw.ingame;
                draw.Arrow(transform.position, targetObject.transform.position);
            }
        }

        public void SetMaxDistance(float value)
        {
            BehaviorTree.SetVariableValue("MaxDistance", value);
        }

        public override void Setup()
        {
            base.Setup();
        }

        public override void Refresh()
        {
            base.Refresh();
            if (Item != null)
            {
                var targetLayers = new List<string>();
                if (Item.hitAllies) targetLayers.Add("Allies");
                if (Item.hitEnemies) targetLayers.Add("Enemies");
                targetMask = LayerMask.GetMask(targetLayers.ToArray());
            }
        }

        public override void ManualAim()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            var target = Physics2D.Raycast(mousePos, Vector2.zero,
                1, targetMask);
            if (target.collider != null)
            {
                if (Vector2.Distance(transform.position, target.transform.position) < Item.maxDistance)
                    BehaviorTree.SetVariableValue("TargetObject", target.collider.gameObject);
            }
            else
            {
                BehaviorTree.SetVariableValue("TargetObject", null);
            }
        }
    }
}