using System;
using Cafeo.Castable;
using Cafeo.Utility;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Cafeo.Templates
{
    public class MeleeSkillTemplate : SkillTemplate
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

        public override UsableItem Generate()
        {
            var item = new MeleeItem(1 + sizeModifier, 1 + rangeModifier);
            CopyBaseParameters(item);
            item.meleeType = meleeType;
            item.utilityType = (UtilityType) 10f;
            // item.active = 0.3f;
            // item.recovery = 0.05f;
            item.AddTag(UsableItem.ItemTag.FreeDPS);
            return item;
        }

        private void Reset()
        {
            hitType = TemplateHitType.HitEnemies;
        }
    }
}