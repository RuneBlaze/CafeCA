using System.Collections;
using Cafeo;
using Cafeo.Utils;

namespace Cafeo.TestItems
{
    public static class SkillPresets
    {
        public static IEnumerator GunnerRegen(BattleVessel user)
        {
            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForRogueSeconds(1);
                user.ApplyHealMp(25);
            }
        }
    }
}