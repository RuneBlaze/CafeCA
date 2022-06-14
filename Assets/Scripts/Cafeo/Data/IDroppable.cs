using Cafeo.Entities;
using UnityEngine;

namespace Cafeo.Data
{
    public interface IDroppable
    {
        public Sprite Icon { get; }
        public Collectable.SizeScale SizeScale { get; }
        public void OnPickedUp(BattleVessel vessel);
        public void OnDrop(BattleVessel owner);

        public virtual bool CanBePickedUp(BattleVessel vessel)
        {
            return true;
        }
    }
}