using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class WearableWeaponTemplate : WearableTemplate
    {
        [BoxGroup("Weapon Specs", centerLabel: true)]
        public SkillTemplate associatedSkill;
    }
}