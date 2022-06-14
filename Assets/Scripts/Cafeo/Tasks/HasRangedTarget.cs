using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cafeo.Aimer;
using UnityEngine;

namespace Cafeo.Tasks
{
    public class HasRangedTarget : Conditional
    {
        private AimerGroup _aimer;
        public SharedVariable<GameObject> targetObject;

        public override void OnStart()
        {
            _aimer = GetComponent<AimerGroup>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_aimer.RangedAimer.TargetObject != null)
            {
                targetObject.SetValue(_aimer.RangedAimer.TargetObject);
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}