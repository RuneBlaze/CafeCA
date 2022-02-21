using BehaviorDesigner.Runtime.Tasks;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class IsPeaceful : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }
    }
}