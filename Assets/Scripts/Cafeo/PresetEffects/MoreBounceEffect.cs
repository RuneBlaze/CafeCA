using Cafeo.Castable;
using Cafeo.Data;
using UnityEngine;

namespace Cafeo.PresetEffects
{
    public class MoreBounceEffect : PresetPassiveEffect
    {
        public MoreBounceEffect(Vector4 userData) : base(userData)
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
            if (!projectile.isMelee)
            {
                projectile.IncBounce(Mathf.RoundToInt(userData[0]));
                projectile.IncPierce(Mathf.RoundToInt(userData[1]));
                if (projectile.deltaSize > 0) projectile.deltaSize += userData[2];
            }
        }

        public override void InfluenceSkill(UsableItem item)
        {
        }
    }
}