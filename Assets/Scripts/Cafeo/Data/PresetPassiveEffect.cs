using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.Data
{
    public abstract class PresetPassiveEffect : IPassiveEffect
    {
        public Vector4 userData;
        // public static Dictionary<string, PresetPassiveEffect> presets = new();

        public PresetPassiveEffect(Vector4 userData)
        {
            this.userData = userData;
        }

        public static PresetPassiveEffect FromPreset(string presetName, Vector4 userData)
        {
            return null;
        }

        public abstract void InitMyself(BattleVessel owner);
        public abstract void TearDown(BattleVessel owner);
        public abstract void EveryTick(BattleVessel owner);
        public abstract void EverySec(BattleVessel owner);
    }
}