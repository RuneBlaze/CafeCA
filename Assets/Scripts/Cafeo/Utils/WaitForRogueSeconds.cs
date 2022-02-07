using UnityEngine;

namespace Cafeo.Utils
{
    public class WaitForRogueSeconds : CustomYieldInstruction
    {
        public override bool keepWaiting => _timer < seconds;

        public float seconds;
        private float _timer;

        public WaitForRogueSeconds(float seconds)
        {
            this.seconds = seconds;
            RogueManager.Instance.rogueUpdateEvent.AddListener(Handler);
        }

        private void Handler()
        {
            _timer += Time.deltaTime;
            if (!keepWaiting)
            {
                RogueManager.Instance.rogueUpdateEvent.RemoveListener(Handler);
            }
        }
        
    }
}