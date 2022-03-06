using BehaviorDesigner.Runtime.Tasks;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class MarkBusy : Action
    {
        public bool busy;
        private GenericBrain brain;

        public override void OnStart()
        {
            base.OnStart();
            brain = GetComponent<GenericBrain>();
        }

        public override TaskStatus OnUpdate()
        {
            brain.busy = busy;
            return TaskStatus.Success;
        }
    }
}