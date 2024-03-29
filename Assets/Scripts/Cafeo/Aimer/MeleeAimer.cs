﻿using Cafeo.Castable;
using Drawing;

namespace Cafeo.Aimer
{
    public class MeleeAimer : GenericAimer<MeleeItem>
    {
        public override MeleeItem Item { get; set; }

        public override void Update()
        {
            base.Update();
            // we draw the aimer using ALINE
            if (Item == null || hidden) return;
            // first, draw a line from center to target coord
            var targetCoord = transform.position + Item.distance * transform.right;
            var draw = Draw.ingame;
            draw.Arrow(transform.position, targetCoord);
        }
    }
}