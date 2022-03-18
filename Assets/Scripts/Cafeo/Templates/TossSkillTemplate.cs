using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class TossSkillTemplate : SkillTemplate
    {
        [BoxGroup("Ranged Specification", centerLabel: true)]
        public float explodeRange;
        
        [BoxGroup("Ranged Specification", centerLabel: true)]
        public float maxDistance;

        [BoxGroup("Ranged Specification", centerLabel: true)]
        public bool alwaysSplash;
    }
}