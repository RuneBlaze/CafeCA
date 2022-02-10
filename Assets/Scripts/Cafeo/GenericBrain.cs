using System;
using BehaviorDesigner.Runtime;
using Cafeo.Aimer;
using Cafeo.Castable;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Cafeo
{
    [RequireComponent(typeof(BattleVessel))]
    public abstract class GenericBrain : MonoBehaviour
    {
        private BehaviorTree behaviorTree;
        public BehaviorTree BehaviorTree => behaviorTree;
        private AimerGroup _aimer;
        public abstract BattleVessel Vessel { get; set; }
        public RogueManager Scene => RogueManager.Instance;
        public AgentSoul Soul => Vessel.soul;
        public abstract void DecideAction();

        public virtual void Start()
        {
            
            _aimer = GetComponent<AimerGroup>();
            if (!Vessel.IsPlayer)
            {
                behaviorTree = gameObject.AddComponent<BehaviorTree>();
                behaviorTree.StartWhenEnabled = false;
                // _behaviorTree.
                if (Vessel.IsAlly)
                {
                    behaviorTree.ExternalBehavior = 
                        Addressables.LoadAssetAsync<ExternalBehaviorTree>("Assets/Data/BehaviorTrees/DefaultAlly.asset").WaitForCompletion();
                }
            
                if (Vessel.IsEnemy)
                {
                    behaviorTree.ExternalBehavior = 
                        Addressables.LoadAssetAsync<ExternalBehaviorTree>("Assets/Data/BehaviorTrees/DefaultEnemy.asset").WaitForCompletion();
                }
                behaviorTree.EnableBehavior();
            }
            else
            {
                behaviorTree = gameObject.AddComponent<BehaviorTree>();
                behaviorTree.StartWhenEnabled = false;
                behaviorTree.ExternalBehavior =
                    Addressables.LoadAssetAsync<ExternalBehaviorTree>("Assets/Data/BehaviorTrees/LeaderFollow.asset").WaitForCompletion();
            }

            if (Vessel.IsAlly)
            {
                Scene.allyBehaviorTrees.Add(behaviorTree);
            }
            else
            {
                Scene.enemyBehaviorTrees.Add(behaviorTree);
            }

            if (Vessel.IsAlly)
            {
                if (!Vessel.IsPlayer)
                {
                    BehaviorTree.SetVariableValue("PreferredDistance", 0.5f);
                    BehaviorTree.SetVariableValue("MaxDistance", 0.7f);
                }
                BehaviorTree.SetVariableValue("TargetTag", Vessel.IsAlly ? "Enemy" : "Ally");
            }
            SetupAimer();
        }

        public void SetupAimer()
        {
            _aimer.SetupTargetTag(Vessel.IsAlly ? "Enemy" : "Ally");
        }

        public void SwitchToItemSatisfying(Predicate<UsableItem> pred)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Vessel.hotbar[i] == null) continue;
                if (pred.Invoke(Vessel.hotbar[i]))
                {
                    Vessel.TrySetHotboxPointer(i);
                    return;
                }
            }
        }

        public void SwitchToFirstMeleeItem()
        {
            SwitchToItemSatisfying(it => it is MeleeItem);
        }

        public void SwitchToFirstRangedItem()
        {
            SwitchToItemSatisfying(it => it is RangedItem);
        }

        public void TrySwitchingToItem(UsableItem.ItemTag itemTag)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Vessel.hotbar[i] == null) continue;
                if (Vessel.hotbar[i].HasTag(itemTag))
                {
                    Vessel.TrySetHotboxPointer(i);
                    return;
                }
            }
        }

        public bool ShouldUseCurrentItem()
        {
            var item = Vessel.RetrieveCurItem();
            if (item == null) return false;
            if (_aimer.CalcTargetObject(item))
            {
                return true;
            }
            return false;
        }
    }
}