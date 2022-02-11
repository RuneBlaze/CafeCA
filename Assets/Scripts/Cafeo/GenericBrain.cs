using System;
using System.Collections;
using System.Collections.Generic;
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

        private Queue<QueuedAction> actionQueue;

        public virtual void Start()
        {
            actionQueue = new Queue<QueuedAction>();
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

        public void SwitchToItemSatisfying(Predicate<UsableItem> pred, out UsableItem item)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Vessel.hotbar[i] == null) continue;
                if (pred.Invoke(Vessel.hotbar[i]))
                {
                    Vessel.TrySetHotboxPointer(i);
                    item = Vessel.hotbar[i];
                    return;
                }
            }
            item = null;
        }

        public void SwitchToItemSatisfying(Predicate<UsableItem> pred)
        {
            UsableItem item;
            SwitchToItemSatisfying(pred, out item);
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

        public bool HasPositiveUtility(UsableItem item)
        {
            return true;
        }

        public void QueueItemOfTag(UsableItem.ItemTag itemTag)
        {
            actionQueue.Enqueue(new QueuedAction.UseItemOfType(itemTag));
        }

        public void ClearQueue()
        {
            actionQueue.Clear();
        }

        public void InterpretQueue()
        {
            if (actionQueue.Count == 0) return;
            var action = actionQueue.Peek();
            bool performedAction = false;
            switch (action)
            {
                case QueuedAction.UseItemOfType useItemOfType:
                    UsableItem item;
                    SwitchToItemSatisfying(it => it.HasTag(useItemOfType.tag), out item);
                    if (item == null)
                    {
                        // don't have good item satisfying the condition, we should just throw this away
                        performedAction = true;
                    }
                    else
                    {
                        // have a good item, if we can use it, use it. If we cannot use it, wait
                        if (HasPositiveUtility(item))
                        {
                            Vessel.ActivateItem(item);
                            performedAction = true;
                        }
                        else
                        {
                            // now we wait...
                        }
                    }
                    break;
            }
            if (performedAction) actionQueue.Dequeue();
            
        }
    }
}