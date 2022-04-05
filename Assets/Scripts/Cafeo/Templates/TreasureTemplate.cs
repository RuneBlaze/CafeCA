using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class TreasureTemplate : WithIcon, ITemplate<Treasure>
    {
        [BoxGroup("Effects", centerLabel: true)]
        public HitEffects leaderEffect;
        [BoxGroup("Effects", centerLabel: true)]
        public HitEffects otherEffects;
        [BoxGroup("Effects", centerLabel: true)]
        public SkillTemplate leaderSkill;
        [BoxGroup("Effects", centerLabel: true)]
        public SkillTemplate nonLeaderSkill;

        public Treasure Generate()
        {
            var treasure = 
                new Treasure(displayName, leaderEffect, otherEffects, 
                    leaderSkill?.Generate(), nonLeaderSkill?.Generate(), icon);
            return treasure;
        }
    }
}