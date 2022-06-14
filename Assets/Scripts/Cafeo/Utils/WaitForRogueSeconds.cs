using UnityEngine;

namespace Cafeo.Utils
{
    public class WaitForRogueSeconds : CustomYieldInstruction
    {
        private float _timer;

        public float seconds;

        public WaitForRogueSeconds(float seconds)
        {
            this.seconds = seconds;
            RogueManager.Instance.rogueUpdateEvent.AddListener(Handler);
        }

        public override bool keepWaiting => _timer < seconds;

        private void Handler()
        {
            _timer += Time.deltaTime;
            if (!keepWaiting) RogueManager.Instance.rogueUpdateEvent.RemoveListener(Handler);
        }
    }
}