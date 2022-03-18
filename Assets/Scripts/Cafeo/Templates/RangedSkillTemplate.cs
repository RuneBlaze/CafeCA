using Cafeo.Castable;
using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class RangedSkillTemplate : SkillTemplate
    {
        [BoxGroup("Ranged Specification", centerLabel: true)]
        public int shots = 1;

        [BoxGroup("Ranged Specification", centerLabel: true)]
        public int fan = 0;

        [BoxGroup("Ranged Specification", centerLabel: true)]
        [ProgressBar(0, 360)]
        public int spread = 0;
        
        [BoxGroup("Ranged Specification", centerLabel: true)]
        public float duration = 0;
        
        [BoxGroup("Ranged Specification", centerLabel: true)]
        public float instability = 0;

        [BoxGroup("Ranged Specification", centerLabel: true)]
        public bool withPrimaryShot;
    }
}