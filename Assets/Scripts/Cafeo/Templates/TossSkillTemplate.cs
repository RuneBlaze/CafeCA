using System;
using Cafeo.Castable;
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

        public override UsableItem Generate()
        {
            var item = new TossItem();
            CopyBaseParameters(item);
            item.radius = explodeRange;
            item.maxDistance = maxDistance;
            item.alwaysSplash = alwaysSplash;
            return item;
        }

        private void Reset()
        {
            hitType = TemplateHitType.HitAllies;
        }
    }
}