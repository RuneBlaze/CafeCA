using System;
using System.Collections.Generic;
using System.Linq;
using Cafeo.Utils;
using Drawing;
using Unity.Mathematics;
using UnityEngine;
using static Cafeo.Utils.ImDraw;
using static Unity.Mathematics.math;

namespace Cafeo
{
    public class AllyParty : Singleton<AllyParty>
    {
        public RogueManager Scene => RogueManager.Instance;

        private void LateUpdate()
        {
            var draw = Draw.ingame;
            var hpBg = Palette.purple;
            var hpFg = Palette.red;
            var mpBg = Palette.deepBlue;
            var mpFg = Palette.skyBlue;

            var y = 650;
            using (draw.InScreenSpace(Camera.main))
            {
                draw.SolidRectangle(new Rect(new Rect(40,y - 20,240,4 * 135+10)), Palette.SwapAlpha(Palette.black, 0.5f));
                foreach (var vessel in Scene.Allies().Reverse())
                {
                    var soul = vessel.soul;
                    draw.Label2D(float3(50, y + 90, 0), soul.DisplayName, 55f, Palette.milkWhite);
                    DrawGauge(draw, 150, y + 90, 50, 15, (int) (vessel.dashTimer * 50), 150, hpBg, hpFg);
                    DrawGauge(draw, 50, y + 40, 200, 15, soul.hp, soul.MaxHp, hpBg, hpFg);
                    DrawGauge(draw, 50, y + 10, 200, 15, soul.mp, soul.MaxMp, mpBg, mpFg);
                    y += 135;
                }
            }
        }
    }
}