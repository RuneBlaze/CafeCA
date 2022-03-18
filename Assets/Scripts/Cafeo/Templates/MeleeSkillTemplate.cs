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
    }
}