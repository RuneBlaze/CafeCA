using Cafeo.Castable;
using Cafeo.Data;
using UnityEngine;

namespace Cafeo.PresetEffects
{
    public class MoreSpreadEffect : PresetPassiveEffect
    {
        public MoreSpreadEffect(Vector4 userData) : base(userData)
        {
        }

        public override void InitMyself(BattleVessel owner)
        {
            
        }

        public override void TearDown(BattleVessel owner)
        {
            
        }

        public override void EveryTick(BattleVessel owner)
        {
            
        }

        public override void EverySec(BattleVessel owner)
        {
            
        }

        public override void InfluenceProjectile(Projectile projectile)
        {
            
        }

        public override void InfluenceSkill(UsableItem item)
        {
            int shotsPlus = Mathf.RoundToInt(userData[0]);
            int fanPlus = Mathf.RoundToInt(userData[1]);
            float durationSlope = userData[2];
            if (item is RangedItem rangedItem)
            {
                rangedItem.shotMod += shotsPlus;
                rangedItem.fanMod += fanPlus;
                if (rangedItem.EffectiveDuration <= 0)
                {
                    // FIXME: fix the duration calculation
                    rangedItem.durationMod = Mathf.Max(0.05f,rangedItem.oldActive * (1 + (0.5f * (rangedItem.EffectiveShots - 1))));
                }

                if (rangedItem.EffectiveSpread <= 0)
                {
                    rangedItem.spreadMod += 90;
                }
                // rangedItem. += durationSlope;
            }
        }
    }
}