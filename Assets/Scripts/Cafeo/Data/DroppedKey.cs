using Cafeo.Entities;
using UnityEngine;

namespace Cafeo.Data
{
    public class DroppedKey : IDroppable
    {
        public Sprite Icon => RogueManager.Instance.keySprite;
        public Collectable.SizeScale SizeScale => Collectable.SizeScale.Medium;
        public void OnPickedUp(BattleVessel vessel)
        {
            AllyParty.Instance.GainKeys(1);
        }

        public void OnDrop(BattleVessel owner)
        {
            throw new System.NotImplementedException();
        }
    }
}