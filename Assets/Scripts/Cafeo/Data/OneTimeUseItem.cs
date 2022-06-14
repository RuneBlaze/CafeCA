using Cafeo.Castable;
using Cafeo.Entities;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo.Data
{
    public class OneTimeUseItem : UsableItem, IDroppable
    {
        public int charges;
        public Sprite iconOverride;
        public int maxCharges;
        public UsableItem underlying;

        public OneTimeUseItem(UsableItem underlying, int maxCharges, Sprite iconOverride)
        {
            this.underlying = underlying;
            this.maxCharges = maxCharges;
            this.iconOverride = iconOverride;
            icon = iconOverride;
            Assert.IsNotNull(iconOverride);
            charges = maxCharges;
        }

        public OneTimeUseItem(UsableItem underlying, int maxCharges) : this(underlying, maxCharges, underlying.icon)
        {
        }

        public override bool ShouldDiscard => charges <= 0;

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

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            underlying.OnUse(user);
            charges--;
        }
    }
}