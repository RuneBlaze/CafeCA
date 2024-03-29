﻿using BehaviorDesigner.Runtime.Tasks;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class MeleeApproach : GenericApproach
    {
        // [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is moving towards")]
        // public SharedGameObject target;
        // [BehaviorDesigner.Runtime.Tasks.Tooltip("If target is null then use the target position")]
        // public SharedVector3 targetPosition;
        // [BehaviorDesigner.Runtime.Tasks.Tooltip("The distance that the agent is within to be considered 'close enough' to the target")]
        // public SharedFloat preferredDistance;
        // public SharedFloat maxRunningTime;

        // private BattleVessel vessel;
        // private int mask;
        // public SharedFloat zoningSkillInterval;
        // private float timer;


        // public override void OnStart()
        // {
        //     base.OnStart();
        //     vessel = GetComponent<BattleVessel>();
        //     SetDestination(Target());
        //     mask = LayerMask.GetMask("Ignore Raycast", "Allies", "HitAlly", "HitBoth", "HitEnemy");
        //     timer = 0;
        //     vessel.Brain.ClearQueue();
        //     vessel.Brain.SwitchToItemSatisfying(it => it.HasTag(UsableItem.ItemTag.Approach));
        // }


        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        // In general, we want to move towards the target until we have a clear sight of the
        // target at some "preferred distance", at which point it is "success".
        // but also, for each interval, we want to queue in a "zoning" skill.
        // public override TaskStatus OnUpdate()
        // {
        //     if (target.Value == null)
        //     {
        //         return TaskStatus.Success;
        //     }
        //     bool inSight = MovementUtility.LineOfSight(transform,
        //         Vector3.zero, target.Value,
        //         Vector3.zero, true, mask, true);
        //     var rm = Scene;
        //     var lhs = rm.GetVesselFromGameObject(gameObject);
        //     var rhs = rm.GetVesselFromGameObject(target.Value);
        //     var distance = lhs.BodyDistance(rhs);
        //     if (inSight && distance < preferredDistance.Value || HasArrived()) {
        //         return TaskStatus.Success;
        //     }
        //     timer += Time.deltaTime;
        //     if (timer > zoningSkillInterval.Value)
        //     {
        //         UseZoningSkill();
        //         timer = 0;
        //     }
        //     SetDestination(Target());
        //     return TaskStatus.Running;
        // }

        // private void UseZoningSkill()
        // {
        //     vessel.Brain.QueueItemOfTag(UsableItem.ItemTag.Approach);
        // }
        //
        // // Return targetPosition if target is null
        // private Vector3 Target()
        // {
        //     if (target.Value != null) {
        //         return target.Value.transform.position;
        //     }
        //     return targetPosition.Value;
        // }

        // public RogueManager Scene => RogueManager.Instance;

        // public override void OnReset()
        // {
        //     base.OnReset();
        //     target = null;
        //     targetPosition = Vector3.zero;
        //     zoningSkillInterval.SetValue(3);
        //     maxRunningTime.SetValue(99);
        //     // Scene.rogueUpdateEvent.RemoveListener(OnRogueUpdate);
        // }
    }
}