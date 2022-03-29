using Cafeo.Data;
using Cafeo.Entities;
using UnityEngine;

namespace Cafeo
{
    // 宝物
    public class Treasure : IDroppable
    {
        public BattleVessel owner;
        public string displayName;
        public bool LeaderEquipped => true;
        public RogueManager Scene => RogueManager.Instance;
        public AbstractItem leaderSkill;
        public AbstractItem nonLeaderSkill;

        public Treasure(string displayName, AbstractItem leaderSkill, AbstractItem nonLeaderSkill, Sprite icon)
        {
            this.displayName = displayName;
            this.leaderSkill = leaderSkill;
            this.nonLeaderSkill = nonLeaderSkill;
            Icon = icon;
        }

        public virtual void OnEquip()
        {
            
        }

        public virtual void OnUnequip()
        {
            
        }

        public Sprite Icon { get; private set; }
        public Collectable.SizeScale SizeScale => Collectable.SizeScale.Large;
        public void OnPickedUp(BattleVessel vessel)
        {
            throw new System.NotImplementedException();
        }
    }
}