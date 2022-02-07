using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject;
using UnityEngine;

namespace Cafeo.Tasks
{
    public class MeleeApproach : IAstarAIMovement
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is moving towards")]
        public SharedGameObject target;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        private BattleVessel _vessel;

        public override void OnStart()
        {
            base.OnStart();
            _vessel = GetComponent<BattleVessel>();
            
            SetDestination(Target());
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (HasArrived()) {
                return TaskStatus.Success;
            }

            SetDestination(Target());

            return TaskStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (target.Value != null) {
                return target.Value.transform.position;
            }
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
        }
    }
}