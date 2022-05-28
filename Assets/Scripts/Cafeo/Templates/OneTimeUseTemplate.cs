using System;
using Cafeo.Data;
using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    // denotes one time use items
    public class OneTimeUseTemplate : WithIcon, ITemplate<OneTimeUseItem>
    {
        [BoxGroup("Skill", centerLabel: true)] public SkillTemplate skill;
        [BoxGroup("Skill", centerLabel: true)] public int charges;
        public OneTimeUseItem Generate()
        {
            var sk = skill.Generate();
            var item = new OneTimeUseItem(sk, charges, icon == null ? sk.icon : icon)
            {
                name = displayName
            };
            return item;
        }

        private void Reset()
        {
            charges = 6;
        }
    }
}