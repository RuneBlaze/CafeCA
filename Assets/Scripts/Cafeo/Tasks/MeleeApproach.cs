using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject;
using Cafeo.Castable;
using UnityEngine;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class MeleeApproach : IAstarAIMovement
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is moving towards")]
        public SharedGameObject target;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;
        public SharedFloat preferredDistance;
        public SharedFloat maxRunningTime;
        private BattleVessel _vessel;
        private int _mask;
        public SharedFloat zoningSkillInterval;
        private float _timer;
        

        public override void OnStart()
        {
            base.OnStart();
            _vessel = GetComponent<BattleVessel>();
            SetDestination(Target());
            _mask = LayerMask.GetMask("Ignore Raycast", "Allies", "HitAlly", "HitBoth", "HitEnemy");
            _timer = 0;
            _vessel.Brain.ClearQueue();
            _vessel.Brain.SwitchToItemSatisfying(it => it.HasTag(UsableItem.ItemTag.Approach));
            // Scene.rogueUpdateEvent.AddListener(OnRogueUpdate);
        }

        // private void OnRogueUpdate()
        // {
        //     _timer += Time.deltaTime;
        // }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        
        // In general, we want to move towards the target until we have a clear sight of the
        // target at some "preferred distance", at which point it is "success".
        
        // but also, for each interval, we want to queue in a "zoning" skill.
        public override TaskStatus OnUpdate()
        {
            if (target.Value == null)
            {
                return TaskStatus.Success;
            }
            bool inSight = MovementUtility.LineOfSight(transform,
                Vector3.zero, target.Value,
                Vector3.zero, true, _mask, true);
            var rm = RogueManager.Instance;
            var lhs = rm.GetVesselFromGameObject(gameObject);
            var rhs = rm.GetVesselFromGameObject(target.Value);
            var distance = lhs.BodyDistance(rhs);
            if (inSight && distance < preferredDistance.Value || HasArrived()) {
                return TaskStatus.Success;
            }
            _timer += Time.deltaTime;
            if (_timer > zoningSkillInterval.Value)
            {
                UseZoningSkill();
                _timer = 0;
            }
            SetDestination(Target());
            return TaskStatus.Running;
        }

        private void UseZoningSkill()
        {
            _vessel.Brain.QueueItemOfTag(UsableItem.ItemTag.Approach);
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (target.Value != null) {
                return target.Value.transform.position;
            }
            return targetPosition.Value;
        }

        public RogueManager Scene => RogueManager.Instance;

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
            zoningSkillInterval.SetValue(3);
            maxRunningTime.SetValue(99);
            // Scene.rogueUpdateEvent.RemoveListener(OnRogueUpdate);
        }
    }
}