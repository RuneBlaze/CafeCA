using System;
using BehaviorDesigner.Runtime;
using Cafeo.Castable;
using Cafeo.Data;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo.Aimer
{
    public class AimerGroup : MonoBehaviour
    {
        public bool autoAim;
        public bool locked;
        private MeleeAimer meleeAimer;

        public RangedAimer RangedAimer { get; private set; }

        public TossAimer TossAimer { get; private set; }

        public void Awake()
        {
            RangedAimer = GetComponentInChildren<RangedAimer>();
            RangedAimer.Setup();
            RangedAimer.Hide();

            meleeAimer = GetComponentInChildren<MeleeAimer>();
            meleeAimer.Setup();
            meleeAimer.Hide();

            TossAimer = GetComponentInChildren<TossAimer>();
            TossAimer.Setup();
            TossAimer.Hide();

            autoAim = true;
        }

        private void Update()
        {
            RangedAimer.autoAim = autoAim;
            meleeAimer.autoAim = autoAim;
            TossAimer.autoAim = autoAim;

            RangedAimer.locked = locked;
            meleeAimer.locked = locked;
            TossAimer.locked = locked;
        }

        public void RequestAimer(UsableItem item)
        {
            RangedAimer.Hide();
            meleeAimer.Hide();
            TossAimer.Hide();
            var targetTag = item.targetTag;
            switch (item)
            {
                case RangedItem rangedItem:
                    RangedAimer.SetupTargetTag(targetTag);
                    RangedAimer.Show();
                    RangedAimer.Item = rangedItem;
                    RangedAimer.Refresh();
                    break;
                case MeleeItem meleeItem:
                    meleeAimer.SetupTargetTag(targetTag);
                    meleeAimer.Show();
                    meleeAimer.Item = meleeItem;
                    meleeAimer.Refresh();
                    break;
                case TossItem tossItem:
                    TossAimer.SetupTargetTag(targetTag);
                    TossAimer.SetMaxDistance(tossItem.maxDistance);
                    TossAimer.Show();
                    TossAimer.Item = tossItem;
                    TossAimer.Refresh();
                    break;
                case OneTimeUseItem oneTimeUseItem:
                    RequestAimer(oneTimeUseItem.underlying);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item, null);
            }
        }

        public void SetupTargetTag(string targetTag)
        {
            RangedAimer.Setup();
            RangedAimer.SetupTargetTag(targetTag);
        }

        public Vector2 CalcDirection(UsableItem item)
        {
            return item switch
            {
                RangedItem rangedItem => RangedAimer.transform.right,
                MeleeItem => meleeAimer.transform.right,
                TossItem => TossAimer.transform.right,
                _ => RangedAimer.transform.right
            };
        }

        public BehaviorTree GetBehaviourTree(UsableItem item)
        {
            return item switch
            {
                RangedItem rangedItem => RangedAimer.BehaviorTree,
                MeleeItem => meleeAimer.BehaviorTree,
                TossItem => TossAimer.BehaviorTree,
                _ => null
            };
        }

        public void SetAllTargetObject(GameObject go)
        {
            RangedAimer.BehaviorTree.SetVariableValue("TargetObject", go);
            meleeAimer.BehaviorTree.SetVariableValue("TargetObject", go);
            TossAimer.BehaviorTree.SetVariableValue("TargetObject", go);
        }

        public GameObject CalcTargetObject(UsableItem item)
        {
            var bt = GetBehaviourTree(item);
            return bt != null ? bt.GetVariable("TargetObject").GetValue() as GameObject : null;
        }

        public bool IsAimedAtTargetObject(UsableItem item, float tol = 10)
        {
            var targetObject = CalcTargetObject(item);
            if (targetObject == null) return false;
            var dir = CalcDirection(item);
            Vector2 pos = targetObject.transform.position - transform.position;
            var delta = VectorUtils.DegreesBetween(pos.normalized, dir);
            return Mathf.Abs(delta) < tol;
        }

        public GameObject CalcRangedTarget()
        {
            return RangedAimer.TargetObject;
        }
    }
}