using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
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

        public SharedFloat preferredDistance;

        private BattleVessel _vessel;
        private int _mask;

        public override void OnStart()
        {
            base.OnStart();
            _vessel = GetComponent<BattleVessel>();
            SetDestination(Target());
            _mask = LayerMask.GetMask("Ignore Raycast", "Allies", "HitAlly", "HitBoth", "HitEnemy");
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        
        // In general, we want to move towards the target until we have a clear sight of the
        // target at some "preferred distance", at which point it is "success".
        
        // but also, for each interval, we want to queue in a "zoning" skill.
        public override TaskStatus OnUpdate()
        {
            bool inSight = MovementUtility.LineOfSight(transform,
                Vector3.zero, target.Value,
                Vector3.zero, true, _mask, true);
            var rm = RogueManager.Instance;
            var lhs = rm.GetVesselFromGameObject(gameObject);
            var rhs = rm.GetVesselFromGameObject(target.Value);
            var distance = lhs.BodyDistance(rhs);
            if ((inSight && distance < preferredDistance.Value) || HasArrived()) {
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