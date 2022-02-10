using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Cafeo.Tasks
{
    public class DelayedHasReceivedEvent : Conditional
    {
        [Tooltip("The name of the event to receive")]
        public SharedString eventName = "";
        [Tooltip("Optionally store the first sent argument")]
        [SharedRequired]
        public SharedVariable storedValue1;
        [Tooltip("Optionally store the second sent argument")]
        [SharedRequired]
        public SharedVariable storedValue2;
        [Tooltip("Optionally store the third sent argument")]
        [SharedRequired]
        public SharedVariable storedValue3;

        private bool eventReceived = false;
        private bool registered = false;
        
        private object receivedValue1;
        private object receivedValue2;
        private object receivedValue3;

        public override void OnStart()
        {
            // Let the behavior tree know that we are interested in receiving the event specified
            if (!registered) {
                Owner.RegisterEvent(eventName.Value, ReceivedEvent);
                Owner.RegisterEvent<object>(eventName.Value, ReceivedEvent);
                Owner.RegisterEvent<object, object>(eventName.Value, ReceivedEvent);
                Owner.RegisterEvent<object, object, object>(eventName.Value, ReceivedEvent);
                registered = true;
            }
        }

        public override TaskStatus OnUpdate()
        {
            // return eventReceived ? TaskStatus.Success : TaskStatus.Failure;
            if (eventReceived)
            {
                if (receivedValue1 != null)
                {
                    storedValue1.SetValue(receivedValue1);
                    receivedValue1 = null;
                }
                
                if (receivedValue2 != null)
                {
                    storedValue2.SetValue(receivedValue2);
                    receivedValue2 = null;
                }
                
                if (receivedValue3 != null)
                {
                    storedValue3.SetValue(receivedValue3);
                    receivedValue3 = null;
                }
                
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
        

        public override void OnEnd()
        {
            if (eventReceived) {
                Owner.UnregisterEvent(eventName.Value, ReceivedEvent);
                Owner.UnregisterEvent<object>(eventName.Value, ReceivedEvent);
                Owner.UnregisterEvent<object, object>(eventName.Value, ReceivedEvent);
                Owner.UnregisterEvent<object, object, object>(eventName.Value, ReceivedEvent);
                registered = false;
            }
            eventReceived = false;
        }

        private void ReceivedEvent()
        {
            eventReceived = true;
        }

        private void ReceivedEvent(object arg1)
        {
            ReceivedEvent();

            if (storedValue1 != null && !storedValue1.IsNone)
            {
                receivedValue1 = arg1;
            }
        }

        private void ReceivedEvent(object arg1, object arg2)
        {
            ReceivedEvent();

            if (storedValue1 != null && !storedValue1.IsNone)
            {
                receivedValue1 = arg1;
            }

            if (storedValue2 != null && !storedValue2.IsNone) {
                receivedValue2 = arg2;
            }
        }

        private void ReceivedEvent(object arg1, object arg2, object arg3)
        {
            ReceivedEvent();

            if (storedValue1 != null && !storedValue1.IsNone) {
                receivedValue1 = arg1;
            }

            if (storedValue2 != null && !storedValue2.IsNone) {
                receivedValue2 = arg2;
            }

            if (storedValue3 != null && !storedValue3.IsNone) {
                receivedValue3 = arg3;
            }
        }

        public override void OnBehaviorComplete()
        {
            // Stop receiving the event when the behavior tree is complete
            Owner.UnregisterEvent(eventName.Value, ReceivedEvent);
            Owner.UnregisterEvent<object>(eventName.Value, ReceivedEvent);
            Owner.UnregisterEvent<object, object>(eventName.Value, ReceivedEvent);
            Owner.UnregisterEvent<object, object, object>(eventName.Value, ReceivedEvent);

            eventReceived = false;
            registered = false;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values
            eventName = "";
        }
    }
}