using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cafeo.Castable;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class DealStapleDamage : Action
    {
        private GenericBrain brain;
        public SharedInt maxQueue = 1;
        public bool failureOnDiscard;

        private bool waitingForSkillExecution;
        private bool skillExecuted;
        private bool skillUsed;
        private int queueId;
        private bool itemSucceeded;

        public override void OnStart()
        {
            base.OnStart();
            brain = GetComponent<GenericBrain>();
            
            
            brain.itemUsed.AddListener(i =>
            {
                if (i == queueId)
                {
                    skillUsed = true;
                    itemSucceeded = true;
                }
            });
            
            brain.itemDiscarded.AddListener(i =>
            {
                if (i == queueId)
                {
                    skillUsed = true;
                    itemSucceeded = false;
                }
            });
            
            brain.Vessel.enterState.AddListener(st =>
            {
                if (st == BattleVessel.State.Idle && skillUsed)
                {
                    skillExecuted = true;
                }
            });
        }

        public override TaskStatus OnUpdate()
        {
            if (!waitingForSkillExecution)
            {
                queueId = brain.QueueItemOfTag(UsableItem.ItemTag.FreeDPS, maxQueue.Value);
                waitingForSkillExecution = true;
                if (queueId == -1)
                {
                    return failureOnDiscard ? TaskStatus.Failure : TaskStatus.Success;
                }
                return TaskStatus.Running;
            }
            else
            {
                // now we are waiting
                if (skillExecuted)
                {
                    waitingForSkillExecution = false;
                    skillExecuted = false;
                    skillUsed = false;
                    return failureOnDiscard && !itemSucceeded ? TaskStatus.Failure : TaskStatus.Success;
                }
                return TaskStatus.Running;
            }
        }

        public override void OnReset()
        {
            base.OnReset();
            maxQueue = 1;
        }
    }
}