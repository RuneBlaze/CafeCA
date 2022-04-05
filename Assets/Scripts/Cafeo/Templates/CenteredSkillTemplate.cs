using Cafeo.Castable;
using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class CenteredSkillTemplate : SkillTemplate
    {
        [BoxGroup("Ranged Specification", centerLabel: true)]
        public float explodeRange;
        public override UsableItem Generate()
        {
            var item = new TossItem
            {
                radius = explodeRange,
                alwaysSplash = true,
                maxDistance = 0
            };
            return item;
        }
        
        private void Reset()
        {
            hitType = TemplateHitType.HitAllies;
        }
    }
}