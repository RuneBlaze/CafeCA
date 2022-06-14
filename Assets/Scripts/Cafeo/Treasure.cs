using Cafeo.Data;
using Cafeo.Entities;
using UnityEngine;

namespace Cafeo
{
    // 宝物
    public class Treasure : IDroppable, IStatusTag
    {
        public string displayName;
        public HitEffects leaderEffect;
        public bool leaderEquipped;

        public AbstractItem leaderSkill;
        public HitEffects otherEffects;
        public AbstractItem otherSkill;
        public BattleVessel owner;

        public Treasure(string displayName, HitEffects leaderEffect, HitEffects otherEffects, AbstractItem leaderSkill,
            AbstractItem otherSkill, Sprite icon)
        {
            this.displayName = displayName;
            this.leaderEffect = leaderEffect;
            this.otherEffects = otherEffects;
            this.leaderSkill = leaderSkill;
            this.otherSkill = otherSkill;
            Icon = icon;

            leaderEffect.statusTag = this;
            otherEffects.statusTag = this;
        }

        public RogueManager Scene => RogueManager.Instance;

        //
        // public Treasure(string displayName, AbstractItem leaderSkill, AbstractItem nonLeaderSkill, Sprite icon)
        // {
        //     this.displayName = displayName;
        //     this.leaderSkill = leaderSkill;
        //     this.nonLeaderSkill = nonLeaderSkill;
        //     Icon = icon;
        // }

        public Sprite Icon { get; }
        public Collectable.SizeScale SizeScale => Collectable.SizeScale.Large;

        public void OnPickedUp(BattleVessel vessel)
        {
            if (vessel.IsAlly)
            {
                if (vessel.HasTreasure) vessel.DropTreasure();
                vessel.treasure = this;
                vessel.onGainTreasure.Invoke(this);
                var scene = RogueManager.Instance;
                var leader = scene.leaderAlly.GetComponent<BattleVessel>();
                if (vessel == leader)
                {
                    leaderEquipped = true;
                    leaderEffect.Apply(vessel, vessel);
                    foreach (var ally in scene.Allies()) otherEffects.Apply(leader, ally);
                }
                else
                {
                    otherEffects.Apply(vessel, vessel);
                }
            }
            else
            {
                vessel.drops.AddTreasure(this);
            }
        }

        public void OnDrop(BattleVessel from)
        {
            TearDown(from);
        }

        public bool CompareStatusTag(IStatusTag statusTag)
        {
            return Equals(statusTag);
        }

        public void TearDown(BattleVessel vessel)
        {
            if (vessel.IsAlly)
            {
                var scene = RogueManager.Instance;
                var leader = scene.leaderAlly.GetComponent<BattleVessel>();
                if (vessel == leader)
                {
                    leaderEquipped = true;
                    leaderEffect.TearDown(vessel);
                    foreach (var ally in scene.Allies()) otherEffects.TearDown(ally);
                }
                else
                {
                    otherEffects.TearDown(vessel);
                }
            }
            else
            {
                vessel.drops.treasures.Remove(this);
            }
        }
    }
}