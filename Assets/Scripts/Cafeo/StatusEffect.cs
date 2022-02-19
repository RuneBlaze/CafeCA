using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo
{
    public class StatusEffect
    {
        public BattleVessel owner;
        public float duration;
        private float timer;
        public virtual bool Finished => timer >= duration;

        public StatusEffect(BattleVessel owner, float duration)
        {
            this.owner = owner;
            Assert.IsTrue(duration > 0);
            this.duration = duration;
        }

        public virtual void OnAdd()
        {
            
        }

        public virtual void OnEnd()
        {
            
        }

        public virtual void Update()
        {
            timer += Time.deltaTime;
        }
    }
}