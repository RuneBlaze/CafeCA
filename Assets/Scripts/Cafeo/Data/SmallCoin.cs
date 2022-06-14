using System;
using Cafeo.Entities;
using UnityEngine;

namespace Cafeo.Data
{
    public class SmallCoin : IDroppable
    {
        public Sprite Icon => RogueManager.Instance.coinSprite;
        public Collectable.SizeScale SizeScale => Collectable.SizeScale.Small;

        public void OnPickedUp(BattleVessel vessel)
        {
            // var scene = RogueManager.Instance;
            AllyParty.Instance.GainGold(1);
        }

        public void OnDrop(BattleVessel owner)
        {
            throw new NotImplementedException();
        }
    }
}