using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    // denotes one time use items
    public class OneTimeUseTemplate : WithIcon
    {
        [BoxGroup("Skill", centerLabel: true)] public SkillTemplate skill;
    }
}