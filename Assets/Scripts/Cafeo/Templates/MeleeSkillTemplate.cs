using System;
using Cafeo.Castable;
using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class MeleeSkillTemplate : SkillTemplate, ITemplate<MeleeItem>
    {
        [BoxGroup("Melee Specification", centerLabel: true)]
        public MeleeItem.MeleeType meleeType;

        [BoxGroup("Melee Specification", centerLabel: true)]
        public float sizeModifier;
        
        [BoxGroup("Melee Specification", centerLabel: true)]
        public float speedModifier;
        
        [BoxGroup("Melee Specification", centerLabel: true)]
        public float rangeModifier;

        [BoxGroup("Melee Specification", centerLabel: true)]
        public SkillTemplate additionalSkill;

        public MeleeItem Generate()
        {
            var item = new MeleeItem(2 * sizeModifier, 2 * rangeModifier);
            CopyBaseParameters(item);
            return item;
        }

        private void Reset()
        {
            hitType = TemplateHitType.HitEnemies;
        }
    }
}