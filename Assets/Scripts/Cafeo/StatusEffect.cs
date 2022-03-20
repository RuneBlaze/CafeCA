using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo
{
    public class StatusEffect
    {
        public BattleVessel owner;
        public float duration;
        public float timer;
        private int lastSecond;
        public int maxStack = 1;
        public virtual bool Finished => timer >= duration;

        public float hpChangeSec;
        public float mpChangeSec;
        public float cpChangeSec;

        public float hpPercChangeSec;
        public float mpPercChangeSec;
        public float cpPercChangeSec;

        public float shield;
        public float shieldPerc;

        public float atk;
        public float def;
        public float mat;
        public float mdf;
        public float spd;

        public string displayName;

        public bool paralyzed;

        public HitEffects source;

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
            if (lastSecond < Mathf.FloorToInt(timer))
            {
                lastSecond = Mathf.FloorToInt(timer);
                EverySecond();
            }
        }

        private void EverySecond()
        {
            if (hpChangeSec != 0)
            {
                if (hpChangeSec > 0)
                {
                    owner.ApplyHeal(Mathf.RoundToInt(hpChangeSec));
                }
                else
                {
                    owner.ApplyDamage(Mathf.RoundToInt(-hpChangeSec), 0, Vector2.zero);
                }
            }
            if (mpChangeSec != 0)
            {
                if (mpChangeSec > 0)
                {
                    owner.ApplyHealMp(Mathf.RoundToInt(mpChangeSec));
                }
            }
            if (cpChangeSec != 0)
            {
                if (cpChangeSec > 0)
                {
                    owner.ApplyHealCp(Mathf.RoundToInt(cpChangeSec));
                }
            }
        }
    }
}