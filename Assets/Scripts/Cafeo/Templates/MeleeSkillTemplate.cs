using Cafeo.Castable;
using Sirenix.OdinInspector;

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

        private void Reset()
        {
            hitType = TemplateHitType.HitEnemies;
        }

        public override UsableItem Generate()
        {
            var item = new MeleeItem(1 + sizeModifier, 1 + rangeModifier);
            CopyBaseParameters(item);
            item.meleeType = meleeType;
            item.AddTag(UsableItem.ItemTag.FreeDPS);
            return item;
        }
    }
}