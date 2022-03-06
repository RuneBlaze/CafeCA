using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cafeo.Aimer;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class MarkBusy : Action
    {
        public bool busy;
        public SharedGameObject target;
        private GenericBrain brain;
        private AimerGroup aimerGroup;

        public override void OnStart()
        {
            base.OnStart();
            brain = GetComponent<GenericBrain>();
            aimerGroup = GetComponent<AimerGroup>();
        }

        public override TaskStatus OnUpdate()
        {
            brain.busy = busy;
            if (busy)
            {
                aimerGroup.SetAllTargetObject(target.Value);
            }
            else
            {
                aimerGroup.SetAllTargetObject(null);
            }
            return TaskStatus.Success;
        }
    }
}