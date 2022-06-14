using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using Cafeo.Aimer;
using Cafeo.Castable;
using Cafeo.Utils;
using Pathfinding;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Cafeo
{
    [RequireComponent(typeof(BattleVessel))]
    public abstract class GenericBrain : MonoBehaviour
    {
        public UnityEvent<int> itemUsed;
        public UnityEvent<int> itemDiscarded;

        public bool busy;

        public int
            aggression = 1; // positive: we should approach, negative: we should retreat, zero: we should just zone

        private AimerGroup _aimer;

        private Queue<QueuedAction> actionQueue;
        private AIPath path;
        public BehaviorTree BehaviorTree { get; private set; }

        public abstract BattleVessel Vessel { get; set; }
        public RogueManager Scene => RogueManager.Instance;
        public AgentSoul Soul => Vessel.soul;

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
                BehaviorTree = gameObject.AddComponent<BehaviorTree>();
                BehaviorTree.StartWhenEnabled = false;
                // _behaviorTree.
                if (Vessel.IsAlly)
                    BehaviorTree.ExternalBehavior =
                        Addressables.LoadAssetAsync<ExternalBehaviorTree>("Assets/Data/BehaviorTrees/DefaultAlly.asset")
                            .WaitForCompletion();

                if (Vessel.IsEnemy)
                {
                    var treePath = "Assets/Data/BehaviorTrees/DefaultEnemy.asset";
                    if (!string.IsNullOrEmpty(Vessel.aiType))
                        treePath = $"Assets/Data/BehaviorTrees/EnemyAI/{Vessel.aiType}.asset";
                    BehaviorTree.ExternalBehavior =
                        Addressables.LoadAssetAsync<ExternalBehaviorTree>(treePath).WaitForCompletion();
                }

                BehaviorTree.EnableBehavior();
            }
            else
            {
                BehaviorTree = gameObject.AddComponent<BehaviorTree>();
                BehaviorTree.StartWhenEnabled = false;
                BehaviorTree.ExternalBehavior =
                    Addressables.LoadAssetAsync<ExternalBehaviorTree>("Assets/Data/BehaviorTrees/LeaderFollow.asset")
                        .WaitForCompletion();
            }

            if (Vessel.IsAlly)
                Scene.allyBehaviorTrees.Add(BehaviorTree);
            else
                Scene.enemyBehaviorTrees.Add(BehaviorTree);

            if (Vessel.IsAlly)
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

            if (!Vessel.IsPlayer) BehaviorTree.SetVariableValue("TargetTag", Vessel.IsAlly ? "Enemy" : "Ally");
            SetupAimer();
        }

        public abstract void DecideAction();

        public GameObject RetrieveTargetObject()
        {
            return BehaviorTree != null ? BehaviorTree.GetVariable("TargetObject").GetValue() as GameObject : null;
        }

        public void TrySetTargetObject(GameObject target)
        {
            // Debug.Log("Setting target object to " + target);
            if (BehaviorTree != null)
                // if (RetrieveTargetObject().name != target.name)
                // {
                //     Debug.Log("Setting target to " + target.name);
                // }
                BehaviorTree.SetVariableValue("TargetObject", target);
        }

        public BattleVessel RetrieveTargetEnemy()
        {
            var go = RetrieveTargetObject();
            if (go != null) return Scene.GetVesselFromGameObject(go);
            return null;
        }

        public void SetupAimer()
        {
            _aimer.SetupTargetTag(Vessel.IsAlly ? "Enemy" : "Ally");
        }

        public void SwitchToItemSatisfying(Predicate<UsableItem> pred, out UsableItem item)
        {
            var suitableObjects = HeldItemsWithIdx().Where(it => pred(it.Item1)).ToList();
            if (suitableObjects.Count == 0)
            {
                item = null;
                return;
            }

            var bestItem = suitableObjects.MaxObject(it => CalcUtility(it.Item1));
            if (bestItem.Item2 >= 0)
            {
                Vessel.TrySetHotboxPointer(bestItem.Item2);
                item = Vessel.hotbar[bestItem.Item2];
            }
            else
            {
                item = bestItem.Item1;
            }
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
            for (var i = 0; i < 10; i++)
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
            if (_aimer.CalcTargetObject(item)) return true;
            return false;
        }

        public float CalcUtility(UsableItem item)
        {
            return item.utilityType.CalcUtility(Vessel, item, Vessel.UtilityEnv);
        }

        public bool HasPositiveUtility(UsableItem item)
        {
            var utility = CalcUtility(item);
            // Debug.Log("Utility: " + utility + " for " + item.name);
            return utility > 0;
            // if (item.HasTag(UsableItem.ItemTag.Dash))
            // {
            //     return true;
            // }
            // if (!Vessel.CanUseItem(item)) return false;
            // if (item is MeleeItem meleeItem)
            // {
            //     if (meleeItem.meleeType == MeleeItem.MeleeType.BodyRush)  return true;
            //     var targetObject = _aimer.CalcTargetObject(item);
            //     if (targetObject == null) return false;
            //     if (Scene.GetVesselFromGameObject(targetObject).BodyDistance(Vessel) > 1f) return false;
            //     return  _aimer.IsAimedAtTargetObject(item, 60);
            // }
            // return _aimer.CalcTargetObject(item) != null && _aimer.IsAimedAtTargetObject(item);
        }

        public IEnumerable<UsableItem> HeldItems()
        {
            yield return UsableItem.dashSkill;
            for (var i = 0; i < 10; i++)
            {
                if (Vessel.hotbar[i] == null) continue;
                yield return Vessel.hotbar[i];
            }
        }

        public IEnumerable<(UsableItem, int)> HeldItemsWithIdx()
        {
            yield return (UsableItem.dashSkill, -1);
            for (var i = 0; i < 10; i++)
            {
                if (Vessel.hotbar[i] == null) continue;
                yield return (Vessel.hotbar[i], i);
            }
        }

        public int QueueItemOfTag(UsableItem.ItemTag itemTag, int maxLimit = 50)
        {
            if (actionQueue.Count >= maxLimit) return -1;
            if (HeldItems().All(it => !it.HasTag(itemTag))) return -1;
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
            var performedAction = false;
            var discardedAction = false;
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
                                    // Debug.Log("we are dashing towards the target");
                                    Vessel.TryDash(path.steeringTarget - transform.position);
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
            }
            else if (performedAction)
            {
                var justUsed = actionQueue.Dequeue();
                itemUsed.Invoke(justUsed.id);
            }
        }
    }
}