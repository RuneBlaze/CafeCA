using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject;
using Cafeo.Castable;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class GenericApproach : IAstarAIMovement
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is moving towards")]
        public SharedGameObject target;
        public SharedFloat preferredDistance;
        public SharedFloat tolerance;
        public float timeLimit = -1f;
        protected int mask;
        public SharedFloat zoningSkillInterval;
        
        protected BattleVessel vessel;
        protected float timer;
        protected float atDestinationTimer;

        public override void OnStart()
        {
            base.OnStart();
            InitApproach();
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
            if (target.Value == null)
            {
                return TaskStatus.Failure;
            }
            bool inSight = MovementUtility.LineOfSight(transform,
                Vector3.zero, target.Value,
                Vector3.zero, true, mask, true);
            var rm = Scene;
            var lhs = rm.GetVesselFromGameObject(gameObject);
            var rhs = rm.GetVesselFromGameObject(target.Value);
            var distance = lhs.BodyDistance(rhs);
            if (HasArrived())
            {
                atDestinationTimer += Time.deltaTime;
            }
            else
            {
                atDestinationTimer = 0;
            }

            var pd = preferredDistance.Value;
            if (inSight && (pd > 1 && Mathf.Abs(distance - pd) < 0.5f) || (pd <= 1 && distance < pd) || atDestinationTimer > 0.5f)
            {
                atDestinationTimer = 0;
                return TaskStatus.Success;
            }
            timer += Time.deltaTime;
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
            for (int i = 0; i < 4; i++)
            {
                var diff = targetPos - (Vector2) transform.position;
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
        
        public RogueManager Scene => RogueManager.Instance;
        
        void UseZoningSkill()
        {
            vessel.Brain.QueueItemOfTag(UsableItem.ItemTag.Approach);
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