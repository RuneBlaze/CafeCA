using System.Collections.Generic;
using Cafeo.Data;
using Cafeo.Entities;
using UnityEngine;

namespace Cafeo
{
    public class Charm : IDroppable
    {
        public string displayName;
        public List<HitEffects> passiveEffects;

        public Charm(string displayName, Sprite icon, List<HitEffects> passiveEffects)
        {
            this.displayName = displayName;
            this.passiveEffects = passiveEffects;
            Icon = icon;
        }
        
        public virtual void InitMyself(BattleVessel owner)
        {
            foreach (var passiveEffect in passiveEffects)
            {
                passiveEffect.Apply(owner, owner);
            }
        }

        public virtual void TearDown(BattleVessel owner)
        {
            foreach (var passiveEffect in passiveEffects)
            {
                passiveEffect.TearDown(owner);
            }
        }

        public Sprite Icon { get; private set; }
        public Collectable.SizeScale SizeScale => Collectable.SizeScale.Large;

        public void OnPickedUp(BattleVessel vessel)
        {
            if (vessel.IsAlly)
            {
                AllyParty.Instance.AddCharm(this);
            }
        }
    }
}