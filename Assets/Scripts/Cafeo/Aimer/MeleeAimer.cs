using System;
using System.Drawing;
using BehaviorDesigner.Runtime;
using Cafeo.Castable;
using Drawing;

namespace Cafeo.Aimer
{
    public class MeleeAimer : GenericAimer<MeleeItem>
    {
        public override MeleeItem Item { get; set; }

        public void Update()
        {
            // we draw the aimer using ALINE
            if (Item == null)
            {
                return;
            }
            // first, draw a line from center to target coord
            var targetCoord = transform.position + Item.Distance * transform.right;
            var draw = Draw.ingame;
            draw.Arrow(transform.position, targetCoord);
        }
    }
}