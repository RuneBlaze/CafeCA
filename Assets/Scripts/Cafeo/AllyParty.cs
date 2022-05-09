using System;
using System.Collections.Generic;
using System.Linq;
using Cafeo.Templates;
using Cafeo.Utils;
using Drawing;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using static Cafeo.Utils.ImDraw;
using static Unity.Mathematics.math;

namespace Cafeo
{
    public class AllyParty : Singleton<AllyParty>
    {
        public RogueManager Scene => RogueManager.Instance;
        
        // the resources here are defined to be the stash
        private int gold;

        public int Gold => gold;

        public int Keys => keys;

        private int keys;
        public List<Charm> charms;
        public UnityEvent<Charm> onGainCharm;

        public int ep = 100; // ep multiplied by 10
        public UnityEvent<int> onEpChanged;
        public UnityEvent onStashChanged;

        protected override void Setup()
        {
            base.Setup();
            charms = new List<Charm>();
            onGainCharm = new UnityEvent<Charm>();
            onEpChanged = new UnityEvent<int>();
            onStashChanged = new UnityEvent();
        }

        public void GainKeys(int count)
        {
            keys += count;
            onStashChanged.Invoke();
        }

        public void LoseKeys(int count)
        {
            keys -= count;
            onStashChanged.Invoke();
        }

        public void GainGold(int count)
        {
            gold += count;
            onStashChanged.Invoke();
        }

        public void LoseGold(int count)
        {
            gold -= count;
            onStashChanged.Invoke();
        }

        public void AddCharm(Charm charm)
        {
            charms.Add(charm);
            foreach (var battleVessel in Members())
            {
                charm.InitMyself(battleVessel);
            }
            onGainCharm.Invoke(charm);
        }

        public void BeforeRoom()
        {
            foreach (var ally in Scene.Allies())
            {
                if (!ally.Dead)
                {
                    ally.soul.HealMpPercentage(1);
                }
            }
        }

        public void AfterClear()
        {
            int epCost = 0;
            foreach (var ally in Scene.Allies())
            {
                if (ally.soul.Dead)
                {
                    ally.Revive();
                    epCost += 10;
                }
            }

            if (epCost > 0)
            {
                ep -= epCost;
                onEpChanged.Invoke(ep);
            }
        }

        public IEnumerable<BattleVessel> Members()
        {
            var instance = RogueManager.Instance;
            return instance.Allies();
        }

        private void LateUpdate()
        {
            var draw = Draw.ingame;
            var hpBg = Palette.purple;
            var hpFg = Palette.red;
            var mpBg = Palette.deepBlue;
            var mpFg = Palette.skyBlue;

            var y = 500;
            int itemHeight = 160;
            int dashWidth = 40;
            int N = Scene.Allies().Count();
            using (draw.InScreenSpace(Camera.main))
            {
                draw.SolidRectangle(new Rect(new Rect(40,y - 20,240,4 * itemHeight+10)),
                    Palette.SwapAlpha(Palette.black, 0.5f));
                y += (3 - N) * itemHeight;
                foreach (var vessel in Scene.Allies().Reverse())
                {
                    var soul = vessel.soul;
                    draw.Label2D(float3(50, y + 110, 0), soul.DisplayName, 55f, Palette.milkWhite);
                    DrawGauge(draw, 50, y + 70, 200, 15, soul.hp, soul.MaxHp, hpBg, hpFg);
                    DrawGauge(draw, 50, y + 35, 200, 15, soul.mp, soul.MaxMp, mpBg, mpFg);
                    DrawGauge(draw, 50, y, 140, 15, soul.cp, soul.MaxCp, Palette.green, Palette.yellow);
                    DrawGauge(draw, 250 - dashWidth, y, dashWidth, 15, 
                        (int) (vessel.dashTimer * dashWidth), Mathf.RoundToInt(dashWidth * vessel.MaxDash), Palette.brown, Palette.milkYellow, 0);
                    y += itemHeight;
                }
            }
        }
    }
}