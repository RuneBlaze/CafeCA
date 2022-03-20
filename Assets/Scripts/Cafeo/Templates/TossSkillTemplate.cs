using System;
using Cafeo.Castable;
using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class TossSkillTemplate : SkillTemplate, ITemplate<TossItem>
    {
        [BoxGroup("Ranged Specification", centerLabel: true)]
        public float explodeRange;
        
        [BoxGroup("Ranged Specification", centerLabel: true)]
        public float maxDistance;

        [BoxGroup("Ranged Specification", centerLabel: true)]
        public bool alwaysSplash;

        public TossItem Generate()
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