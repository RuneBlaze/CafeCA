using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cafeo.Castable;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class DealStapleDamage : Action
    {
        private GenericBrain brain;
        public bool failureOnDiscard;
        private bool itemSucceeded;
        public SharedInt maxQueue = 1;
        private int queueId;
        private bool skillExecuted;
        private bool skillUsed;

        private bool waitingForSkillExecution;

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
                    skillExecuted = true;
                }
            });

            brain.itemDiscarded.AddListener(i =>
            {
                if (i == queueId)
                {
                    skillUsed = true;
                    itemSucceeded = false;
                    skillExecuted = true;
                }
            });

            brain.Vessel.enterState.AddListener(st =>
            {
                if (st == BattleVessel.State.Idle && skillUsed) skillExecuted = true;
            });
        }

        public override TaskStatus OnUpdate()
        {
            if (!waitingForSkillExecution)
            {
                queueId = brain.QueueItemOfTag(UsableItem.ItemTag.FreeDPS, maxQueue.Value);
                waitingForSkillExecution = true;
                if (queueId == -1) return failureOnDiscard ? TaskStatus.Failure : TaskStatus.Success;
                return TaskStatus.Running;
            }

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

        public override void OnReset()
        {
            base.OnReset();
            maxQueue = 1;
        }
    }
}