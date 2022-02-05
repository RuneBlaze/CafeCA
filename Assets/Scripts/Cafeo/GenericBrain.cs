using System;
using BehaviorDesigner.Runtime;
using Cafeo.Aimer;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Cafeo
{
    [RequireComponent(typeof(BattleVessel))]
    public abstract class GenericBrain : MonoBehaviour
    {
        private BehaviorTree _behaviorTree;
        private AimerGroup _aimer;
        public abstract BattleVessel Vessel { get; set; }
        public AgentSoul Soul => Vessel.soul;
        public abstract void DecideAction();

        public virtual void Start()
        {
            
            _aimer = GetComponent<AimerGroup>();
            if (!Vessel.IsPlayer)
            {
                _behaviorTree = gameObject.AddComponent<BehaviorTree>();
                _behaviorTree.StartWhenEnabled = false;
                // _behaviorTree.
                if (Vessel.IsAlly)
                {
                    _behaviorTree.ExternalBehavior = 
                        Addressables.LoadAssetAsync<ExternalBehaviorTree>("Assets/Data/BehaviorTrees/DefaultAlly.asset").WaitForCompletion();
                }
            
                if (Vessel.IsEnemy)
                {
                    _behaviorTree.ExternalBehavior = 
                        Addressables.LoadAssetAsync<ExternalBehaviorTree>("Assets/Data/BehaviorTrees/DefaultEnemy.asset").WaitForCompletion();
                }
                _behaviorTree.EnableBehavior();
            }
            SetupAimer();
        }

        public void SetupAimer()
        {
            _aimer.SetupTargetTag(Vessel.IsAlly ? "Enemy" : "Ally");
        }
    }
}