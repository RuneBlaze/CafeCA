using Cafeo.Castable;
using Cafeo.Entities;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo.Data
{
    public class OneTimeUseItem : UsableItem, IDroppable
    {
        public UsableItem underlying;
        public int charges;
        public int maxCharges;
        public Sprite iconOverride;
        public override bool ShouldDiscard => charges <= 0;

        public OneTimeUseItem(UsableItem underlying, int maxCharges, Sprite iconOverride)
        {
            this.underlying = underlying;
            this.maxCharges = maxCharges;
            this.iconOverride = iconOverride;
            this.icon = iconOverride;
            Assert.IsNotNull(iconOverride);
            charges = maxCharges;
        }

        public OneTimeUseItem(UsableItem underlying, int maxCharges) : this(underlying, maxCharges, underlying.icon)
        {
            
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            underlying.OnUse(user);
            charges--;
        }

        public Sprite Icon => icon;
        public Collectable.SizeScale SizeScale => Collectable.SizeScale.Large;
        public void OnPickedUp(BattleVessel vessel)
        {
            vessel.TryGainOneTimeUse(this);
        }

        public void OnDrop(BattleVessel owner)
        {
            // no need to do anything
        }

        public bool CanBePickedUp(BattleVessel vessel)
        {
            return !vessel.IsOneTimeUseFull;
        }
    }
}