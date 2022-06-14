using System.Collections;
using Cafeo.Utils;

namespace Cafeo.TestItems
{
    public static class SkillPresets
    {
        public static IEnumerator GunnerRegen(BattleVessel user)
        {
            for (var i = 0; i < 2; i++)
            {
                yield return new WaitForRogueSeconds(1);
                user.ApplyHealMp(25);
            }
        }
    }
}