using BehaviorDesigner.Runtime.Tasks;
using Cafeo.Castable;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class DealStapleDamage : Action
    {
        private GenericBrain brain;

        public override void OnStart()
        {
            base.OnStart();
            brain = GetComponent<GenericBrain>();
        }

        public override TaskStatus OnUpdate()
        {
            brain.QueueItemOfTag(UsableItem.ItemTag.FreeDPS);
            return TaskStatus.Success;
        }
    }
}