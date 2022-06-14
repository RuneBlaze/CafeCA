using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject;
using Cafeo.Castable;
using Cafeo.Utility;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class GenericApproach : IAstarAIMovement
    {
        protected float atDestinationTimer;
        protected int mask;
        public SharedFloat preferredDistance;

        protected float retargetTimer;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is moving towards")]
        public SharedGameObject target;

        public float timeLimit = -1f;
        protected float timer;
        public SharedFloat tolerance;
        private UtilityEnv utilityEnv;

        protected BattleVessel vessel;
        public SharedFloat zoningSkillInterval;

        public RogueManager Scene => RogueManager.Instance;

        public override void OnStart()
        {
            base.OnStart();
            InitApproach();
            utilityEnv = GetComponent<UtilityEnv>();
        }

        protected virtual void InitApproach()
        {
            vessel = GetComponent<BattleVessel>();
            mask = LayerMask.GetMask("Ignore Raycast", "Allies", "HitAlly", "HitBoth", "HitEnemy");
            timer = 0;
            vessel.Brain.ClearQueue();
            vessel.Brain.SwitchToItemSatisfying(it => it.HasTag(UsableItem.ItemTag.Approach));
        }

        public override TaskStatus OnUpdate()
        {
            if (target.Value == null) return TaskStatus.Failure;
            bool inSight = MovementUtility.LineOfSight(transform,
                Vector3.zero, target.Value,
                Vector3.zero, true, mask, true);
            var rm = Scene;
            var lhs = rm.GetVesselFromGameObject(gameObject);
            var rhs = rm.GetVesselFromGameObject(target.Value);
            var distance = lhs.BodyDistance(rhs);
            if (HasArrived())
                atDestinationTimer += Time.deltaTime;
            else
                atDestinationTimer = 0;

            var pd = preferredDistance.Value;
            if ((inSight && pd > 1 && Mathf.Abs(distance - pd) < 0.5f) || (pd <= 1 && distance < pd) ||
                atDestinationTimer > 0.5f)
            {
                atDestinationTimer = 0;
                return TaskStatus.Success;
            }

            timer += Time.deltaTime;
            retargetTimer += Time.deltaTime;
            if (retargetTimer > 1)
            {
                retargetTimer = 0;
                TryRetargeting();
            }

            if (timeLimit > 0 && timer > timeLimit)
            {
                timer = 0;
                return TaskStatus.Success;
            }

            if (timer > zoningSkillInterval.Value)
            {
                // Debug.Log("Use Zoning Skill");
                UseZoningSkill();
                timer = 0;
            }

            var targetVector = Target();
            SetDestination(targetVector);
            return TaskStatus.Running;
        }

        private Vector2 FindSuitableCloseDistancePoint(Vector2 targetPos, float distance)
        {
            // find a point that is passable while also optimizing for closeness to distance
            // s.t. one can raycast to the targetPos
            Vector2 bestPoint = transform.position;
            float bestScore = 1231234; // find minimum
            for (var i = 0; i < 4; i++)
            {
                var diff = targetPos - (Vector2)transform.position;
                var candidatePoint = targetPos
                                     - VectorUtils.RotateVector(diff.normalized, i * 90) * distance;
                Vector2 actualPoint = SamplePosition(candidatePoint);
                // score = actualPoint's distance to targetPos * 2 + my distance to actualPoint
                var score = (actualPoint - targetPos).magnitude * 2 +
                            ((Vector2)transform.position - actualPoint).magnitude;
                if (score < bestScore)
                {
                    bestScore = score;
                    bestPoint = actualPoint;
                }
            }

            return bestPoint;
        }

        private Vector3 Target()
        {
            if (target.Value != null)
            {
                var targetObject = target.Value;
                var inGrid = FindSuitableCloseDistancePoint(targetObject.transform.position, preferredDistance.Value);
                return inGrid;
            }

            return transform.position;
        }

        private void UseZoningSkill()
        {
            vessel.Brain.QueueItemOfTag(UsableItem.ItemTag.Approach);
        }

        private void TryRetargeting()
        {
            var bestAlternative = utilityEnv.bestTargetEnemy;
            if (bestAlternative != null) vessel.Brain.TrySetTargetObject(bestAlternative.gameObject);
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            zoningSkillInterval.SetValue(3);
            timeLimit = -1;
        }
    }
}