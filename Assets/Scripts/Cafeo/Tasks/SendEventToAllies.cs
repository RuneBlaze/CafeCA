using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo.Tasks
{
    [TaskCategory("Rogue")]
    public class SendEventToAllies : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The event to send")]
        public SharedString eventName;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Optionally specify a first argument to send")]
        [SharedRequired]
        public SharedGameObject argument1;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Optionally specify a second argument to send")]
        [SharedRequired]
        public SharedVariable argument2;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Optionally specify a third argument to send")]
        [SharedRequired]
        public SharedVariable argument3;

        // private BehaviorTree behaviorTree;

        public override void OnStart()
        {

        }

        public override TaskStatus OnUpdate()
        {
            // Send the event and return success
            foreach (var behaviorTree in RogueManager.Instance.allyBehaviorTrees)
            {
                if (argument1 == null || argument1.IsNone) {
                    
                    behaviorTree.SendEvent(eventName.Value);
                } else {
                    if (argument2 == null || argument2.IsNone) {
                        if (argument1.GetValue() != null)
                        {
                            Debug.Log(argument1.Value);
                            behaviorTree.SendEvent<object>(eventName.Value, argument1.GetValue());
                        }
                    } else {
                        if (argument3 == null || argument3.IsNone) {
                            behaviorTree.SendEvent<object, object>(eventName.Value, argument1.GetValue(), argument2.GetValue());
                        } else {
                            behaviorTree.SendEvent<object, object, object>(eventName.Value, argument1.GetValue(), argument2.GetValue(), argument3.GetValue());
                        }
                    }
                }
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values
            eventName = "";
        }
    }
}