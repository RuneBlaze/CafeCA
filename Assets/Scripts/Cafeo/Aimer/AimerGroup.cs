using System;
using BehaviorDesigner.Runtime;
using Cafeo.Castable;
using UnityEngine;

namespace Cafeo.Aimer
{
    public class AimerGroup : MonoBehaviour
    {
        private RangedAimer _rangedAimer;
        private MeleeAimer _meleeAimer;
        private TossAimer _tossAimer;

        public RangedAimer RangedAimer => _rangedAimer;

        public void Awake()
        {
            _rangedAimer = GetComponentInChildren<RangedAimer>();
            _rangedAimer.gameObject.SetActive(false);
            _rangedAimer.Setup();

            _meleeAimer = GetComponentInChildren<MeleeAimer>();
            _meleeAimer.gameObject.SetActive(false);
            _meleeAimer.Setup();

            _tossAimer = GetComponentInChildren<TossAimer>();
            _tossAimer.gameObject.SetActive(false);
            _tossAimer.Setup();
        }

        public void RequestAimer(UsableItem item)
        {
            _rangedAimer.gameObject.SetActive(false);
            _meleeAimer.gameObject.SetActive(false);
            _tossAimer.gameObject.SetActive(false);
            var targetTag = item.targetTag;
            switch (item)
            {
                case RangedItem rangedItem:
                    _rangedAimer.SetupTargetTag(targetTag);
                    _rangedAimer.gameObject.SetActive(true);
                    _rangedAimer.Item = rangedItem;
                    break;
                case MeleeItem meleeItem:
                    _meleeAimer.SetupTargetTag(targetTag);
                    _meleeAimer.gameObject.SetActive(true);
                    _meleeAimer.Item = meleeItem;
                    break;
                case TossItem tossItem:
                    _tossAimer.SetupTargetTag(targetTag);
                    _tossAimer.gameObject.SetActive(true);
                    _tossAimer.Item = tossItem;
                    break;
            }
        }

        public void SetupTargetTag(string targetTag)
        { 
            _rangedAimer.Setup();
           _rangedAimer.SetupTargetTag(targetTag);
        }

        public Vector2 CalcDirection(UsableItem item)
        {
            return item switch
            {
                RangedItem rangedItem => _rangedAimer.transform.right,
                MeleeItem => _meleeAimer.transform.right,
                TossItem => _tossAimer.transform.right,
                _ => _rangedAimer.transform.right
            };
        }

        public BehaviorTree GetBehaviourTree(UsableItem item)
        {
            return (item switch
            {
                RangedItem rangedItem => _rangedAimer.BehaviorTree,
                MeleeItem => _meleeAimer.BehaviorTree,
                TossItem => _tossAimer.BehaviorTree,
                _ => null
            });
        }
        
        public GameObject CalcTargetObject(UsableItem item)
        {
            var bt = GetBehaviourTree(item);
            return bt.GetVariable("TargetObject").GetValue() as GameObject;
        }
    }
}