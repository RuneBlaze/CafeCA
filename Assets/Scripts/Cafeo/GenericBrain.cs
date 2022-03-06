using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Cafeo.Aimer;
using Cafeo.Castable;
using Pathfinding;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

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

        public UnityEvent<int> itemUsed;
        public UnityEvent<int> itemDiscarded;

        public bool busy;

        public int aggression = 1; // positive: we should approach, negative: we should retreat, zero: we should just zone
        private AIPath path;

        private void Awake()
        {
            itemUsed = new UnityEvent<int>();
            itemDiscarded = new UnityEvent<int>();
        }

        public virtual void Start()
        {
            actionQueue = new Queue<QueuedAction>();
            _aimer = GetComponent<AimerGroup>();
            path = GetComponent<AIPath>();
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
                    string treePath = "Assets/Data/BehaviorTrees/DefaultEnemy.asset";
                    if (!string.IsNullOrEmpty(Vessel.aiType))
                    {
                        treePath = $"Assets/Data/BehaviorTrees/EnemyAI/{Vessel.aiType}.asset";
                    }
                    behaviorTree.ExternalBehavior = 
                        Addressables.LoadAssetAsync<ExternalBehaviorTree>(treePath).WaitForCompletion();
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
                    if (Vessel.aiType == "ranged")
                    {
                        BehaviorTree.SetVariableValue("PreferredDistance", 3f);
                        BehaviorTree.SetVariableValue("MaxDistance", 5f);
                        BehaviorTree.SetVariableValue("MinDistance", 1.8f);
                    }
                    else
                    {
                        BehaviorTree.SetVariableValue("PreferredDistance", 0.5f);
                        BehaviorTree.SetVariableValue("MaxDistance", 0.7f);
                        BehaviorTree.SetVariableValue("SkipSight", true);
                    }
                }
            }

            if (!Vessel.IsPlayer)
            {
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
            if (Vessel.IsAlly && pred.Invoke(UsableItem.dashSkill) && Vessel.CanUseItem(UsableItem.dashSkill)) {
                item = UsableItem.dashSkill;
                return;
            }
            for (int i = 0; i < 10; i++)
            {
                if (Vessel.hotbar[i] == null) continue;
                if (pred.Invoke(Vessel.hotbar[i]) && Vessel.CanUseItem(Vessel.hotbar[i]))
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
            if (item.HasTag(UsableItem.ItemTag.Dash))
            {
                return true;
            }
            if (!Vessel.CanUseItem(item)) return false;
            if (item is MeleeItem meleeItem)
            {
                if (meleeItem.meleeType == MeleeItem.MeleeType.BodyRush)  return true;
                var targetObject = _aimer.CalcTargetObject(item);
                if (targetObject == null) return false;
                if (Scene.GetVesselFromGameObject(targetObject).BodyDistance(Vessel) > 1f) return false;
                return  _aimer.IsAimedAtTargetObject(item, 60);
            }
            return _aimer.CalcTargetObject(item) != null && _aimer.IsAimedAtTargetObject(item);
        }

        public int QueueItemOfTag(UsableItem.ItemTag itemTag, int maxLimit = 50)
        {
            if (actionQueue.Count >= maxLimit) return -1;
            var a = new QueuedAction.UseItemOfType(itemTag);
            actionQueue.Enqueue(a);
            return a.id;
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
            bool discardedAction = false;
            switch (action)
            {
                case QueuedAction.UseItemOfType useItemOfType:
                    UsableItem item;
                    SwitchToItemSatisfying(it => it.HasTag(useItemOfType.tag), out item);
                    if (item == null)
                    {
                        // don't have good item satisfying the condition, we should just throw this away
                        discardedAction = true;
                    }
                    else
                    {
                        // have a good item, if we can use it, use it. If we cannot use it, wait
                        if (HasPositiveUtility(item))
                        {
                            if (!item.HasTag(UsableItem.ItemTag.Dash))
                            {
                                Vessel.ActivateItem(item);
                            }
                            else
                            {
                                // we need to handle dashing
                                // var rangedTarget = _aimer.CalcRangedTarget();
                                // if (rangedTarget != null)
                                // {
                                //     // dash towards the target
                                //     // TODO: round dashing to cardinal directions
                                //     var dir = rangedTarget.transform.position - Vessel.transform.position;
                                //     Vessel.TryDash(dir);
                                // }
                                // else
                                // {
                                //     // invalid dash, let's just do nothing
                                //     discardedAction = true;
                                // }
                                // Debug.Log("checking if we should dash");
                                if ((path.destination - transform.position).magnitude >= 1)
                                {
                                    // Debug.Log("we are dashing towards the target");
                                    Vessel.TryDash(path.steeringTarget - transform.position);
                                }
                            }
                            performedAction = true;
                        }
                        else
                        {
                            // now we wait...
                            discardedAction = true;
                        }
                    }
                    break;
            }

            if (discardedAction)
            {
                var justUsed = actionQueue.Dequeue();
                itemDiscarded.Invoke(justUsed.id);
            } else if (performedAction)
            {
                var justUsed = actionQueue.Dequeue();
                itemUsed.Invoke(justUsed.id);
            }
        }
    }
}