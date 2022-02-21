using System;
using BehaviorDesigner.Runtime;
using Cafeo.Castable;
using UnityEngine;

namespace Cafeo.Aimer
{
    public class AimerGroup : MonoBehaviour
    {
        private RangedAimer rangedAimer;
        private MeleeAimer meleeAimer;
        private TossAimer tossAimer;

        public bool autoAim;

        public RangedAimer RangedAimer => rangedAimer;

        public void Awake()
        {
            rangedAimer = GetComponentInChildren<RangedAimer>();
            rangedAimer.Setup();
            rangedAimer.Hide();

            meleeAimer = GetComponentInChildren<MeleeAimer>();
            meleeAimer.Setup();
            meleeAimer.Hide();

            tossAimer = GetComponentInChildren<TossAimer>();
            tossAimer.Setup();
            tossAimer.Hide();
            
            autoAim = true;
        }

        private void Update()
        {
            rangedAimer.autoAim = autoAim;
            meleeAimer.autoAim = autoAim;
            tossAimer.autoAim = autoAim;
        }

        public void RequestAimer(UsableItem item)
        {
            rangedAimer.Hide();
            meleeAimer.Hide();
            tossAimer.Hide();
            var targetTag = item.targetTag;
            switch (item)
            {
                case RangedItem rangedItem:
                    rangedAimer.SetupTargetTag(targetTag);
                    rangedAimer.Show();
                    rangedAimer.Item = rangedItem;
                    rangedAimer.Refresh();
                    break;
                case MeleeItem meleeItem:
                    meleeAimer.SetupTargetTag(targetTag);
                    meleeAimer.Show();
                    meleeAimer.Item = meleeItem;
                    meleeAimer.Refresh();
                    break;
                case TossItem tossItem:
                    tossAimer.SetupTargetTag(targetTag);
                    tossAimer.SetMaxDistance(tossItem.maxDistance);
                    tossAimer.Show();
                    tossAimer.Item = tossItem;
                    tossAimer.Refresh();
                    break;
            }
        }

        public void SetupTargetTag(string targetTag)
        { 
            rangedAimer.Setup();
           rangedAimer.SetupTargetTag(targetTag);
        }

        public Vector2 CalcDirection(UsableItem item)
        {
            return item switch
            {
                RangedItem rangedItem => rangedAimer.transform.right,
                MeleeItem => meleeAimer.transform.right,
                TossItem => tossAimer.transform.right,
                _ => rangedAimer.transform.right
            };
        }

        public BehaviorTree GetBehaviourTree(UsableItem item)
        {
            return (item switch
            {
                RangedItem rangedItem => rangedAimer.BehaviorTree,
                MeleeItem => meleeAimer.BehaviorTree,
                TossItem => tossAimer.BehaviorTree,
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