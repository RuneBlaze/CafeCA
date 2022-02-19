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

        public bool autoAim;

        public RangedAimer RangedAimer => _rangedAimer;

        public void Awake()
        {
            _rangedAimer = GetComponentInChildren<RangedAimer>();
            _rangedAimer.Setup();
            _rangedAimer.Hide();

            _meleeAimer = GetComponentInChildren<MeleeAimer>();
            _meleeAimer.Setup();
            _meleeAimer.Hide();

            _tossAimer = GetComponentInChildren<TossAimer>();
            _tossAimer.Setup();
            _tossAimer.Hide();
            
            autoAim = true;
        }

        private void Update()
        {
            _rangedAimer.autoAim = autoAim;
            _meleeAimer.autoAim = autoAim;
            _tossAimer.autoAim = autoAim;
        }

        public void RequestAimer(UsableItem item)
        {
            _rangedAimer.Hide();
            _meleeAimer.Hide();
            _tossAimer.Hide();
            var targetTag = item.targetTag;
            switch (item)
            {
                case RangedItem rangedItem:
                    _rangedAimer.SetupTargetTag(targetTag);
                    _rangedAimer.Show();
                    _rangedAimer.Item = rangedItem;
                    _rangedAimer.Refresh();
                    break;
                case MeleeItem meleeItem:
                    _meleeAimer.SetupTargetTag(targetTag);
                    _meleeAimer.Show();
                    _meleeAimer.Item = meleeItem;
                    _meleeAimer.Refresh();
                    break;
                case TossItem tossItem:
                    _tossAimer.SetupTargetTag(targetTag);
                    _tossAimer.SetMaxDistance(tossItem.maxDistance);
                    _tossAimer.Show();
                    _tossAimer.Item = tossItem;
                    _tossAimer.Refresh();
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