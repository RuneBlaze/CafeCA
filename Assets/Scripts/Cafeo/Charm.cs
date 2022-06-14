using System.Collections.Generic;
using Cafeo.Data;
using Cafeo.Entities;
using UnityEngine;

namespace Cafeo
{
    public class Charm : IDroppable, IStatusTag
    {
        public string displayName;
        public List<HitEffects> passiveEffects;

        public Charm(string displayName, Sprite icon, List<HitEffects> passiveEffects)
        {
            this.displayName = displayName;
            this.passiveEffects = passiveEffects;
            foreach (var effect in passiveEffects) effect.statusTag = this;
            Icon = icon;
        }

        public Sprite Icon { get; }
        public Collectable.SizeScale SizeScale => Collectable.SizeScale.Large;

        public void OnPickedUp(BattleVessel vessel)
        {
            if (vessel.IsAlly) AllyParty.Instance.AddCharm(this);
        }

        public void OnDrop(BattleVessel owner)
        {
            // charms don't get dropped by allies. No need to tear down
        }

        public bool CompareStatusTag(IStatusTag statusTag)
        {
            return statusTag is Charm charm && charm.displayName == displayName;
        }

        public virtual void InitMyself(BattleVessel owner)
        {
            foreach (var passiveEffect in passiveEffects) passiveEffect.Apply(owner, owner);
        }

        public virtual void TearDown(BattleVessel owner)
        {
            foreach (var passiveEffect in passiveEffects) passiveEffect.TearDown(owner);
        }
    }
}