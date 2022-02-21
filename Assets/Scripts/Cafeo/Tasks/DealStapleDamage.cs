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

        private bool waitingForSkillExecution;
        private bool skillExecuted;

        public override void OnStart()
        {
            base.OnStart();
            brain = GetComponent<GenericBrain>();
            
            brain.Vessel.enterState.AddListener(st =>
            {
                if (st == BattleVessel.State.Idle)
                {
                    skillExecuted = true;
                }
            });
        }

        public override TaskStatus OnUpdate()
        {
            if (!waitingForSkillExecution)
            {
                brain.QueueItemOfTag(UsableItem.ItemTag.FreeDPS, maxQueue.Value);
                waitingForSkillExecution = true;
                return TaskStatus.Running;
            }
            else
            {
                // now we are waiting
                if (skillExecuted)
                {
                    waitingForSkillExecution = false;
                    skillExecuted = false;
                    return TaskStatus.Success;
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