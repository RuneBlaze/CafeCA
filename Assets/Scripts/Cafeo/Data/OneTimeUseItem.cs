using Cafeo.Castable;
using Cafeo.Entities;
using UnityEngine;

namespace Cafeo.Data
{
    public class OneTimeUseItem : UsableItem, IDroppable
    {
        public UsableItem underlying;
        public bool used;
        public override bool ShouldDiscard => used;

        public OneTimeUseItem(UsableItem underlying)
        {
            this.underlying = underlying;
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            underlying.OnUse(user);
            used = true;
        }

        public Sprite Icon => underlying.icon;
        public Collectable.SizeScale SizeScale => Collectable.SizeScale.Large;
        public void OnPickedUp(BattleVessel vessel)
        {
            vessel.TryGainOneTimeUse(this);
        }
        
        public bool CanBePickedUp(BattleVessel vessel)
        {
            return !vessel.IsOneTimeUseFull;
        }
    }
}