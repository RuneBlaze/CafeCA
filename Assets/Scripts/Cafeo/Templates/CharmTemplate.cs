using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public sealed class CharmTemplate : WithIcon, ITemplate<Charm>
    {
        [BoxGroup("Passive Effects", centerLabel: true)]
        public List<HitEffects> passiveEffects;

        public Charm Generate()
        {
            return new Charm(displayName, icon, passiveEffects);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return other is CharmTemplate template &&
                   id == template.id;
        }
    }
}