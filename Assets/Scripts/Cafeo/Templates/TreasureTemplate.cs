using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class TreasureTemplate : WithIcon
    {
        [BoxGroup("Effects", centerLabel: true)]
        public SkillTemplate leaderSkill;
        [BoxGroup("Effects", centerLabel: true)]
        public SkillTemplate nonLeaderSkill;
    }
}