using Cafeo.Entities;
using UnityEngine;

namespace Cafeo.Data
{
    public interface IDroppable
    {
        public Sprite Icon { get; }
        public Collectable.SizeScale SizeScale { get; }
        public void OnPickedUp(BattleVessel vessel);
    }
}