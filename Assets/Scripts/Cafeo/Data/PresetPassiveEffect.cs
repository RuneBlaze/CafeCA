using System;
using System.Collections.Generic;
using Cafeo.Castable;
using Cafeo.PresetEffects;
using UnityEngine;

namespace Cafeo.Data
{
    public abstract class PresetPassiveEffect : IPassiveEffect
    {
        public Vector4 userData;

        public PresetPassiveEffect(Vector4 userData)
        {
            this.userData = userData;
        }

        public static PresetPassiveEffect FromPreset(string presetName, Vector4 userData)
        {
            return presetName switch
            {
                "generic_ranged" => new MoreSpreadEffect(userData),
                "generic_projectile" => new MoreBounceEffect(userData),
                _ => throw new NotImplementedException($"{presetName} is not a valid preset name")
            };
        }

        public abstract void InitMyself(BattleVessel owner);
        public abstract void TearDown(BattleVessel owner);
        public abstract void EveryTick(BattleVessel owner);
        public abstract void EverySec(BattleVessel owner);
        public abstract void InfluenceProjectile(Projectile projectile);
        public abstract void InfluenceSkill(UsableItem item);
    }
}