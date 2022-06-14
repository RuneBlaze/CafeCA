using Cafeo.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo
{
    public class StatusEffect
    {
        public float atk;
        public float cpChangeSec;
        public float cpPercChangeSec;
        public float def;

        public string displayName;
        public float duration;
        public bool eternal;

        public float hpChangeSec;

        public float hpPercChangeSec;
        private int lastSecond;
        public float mat;
        public int maxStack = 1;
        public float mdf;
        public float mpChangeSec;
        public float mpPercChangeSec;
        public BattleVessel owner;

        public bool paralyzed;
        public IPassiveEffect passiveEffect;

        public float shield;
        public float shieldPerc;

        public HitEffects source;
        public float spd;
        public IStatusTag statusTag;
        public float timer;

        public StatusEffect(BattleVessel owner, float duration, bool eternal = false)
        {
            this.owner = owner;
            this.eternal = eternal;
            Assert.IsTrue(duration > 0 || eternal);
            if (eternal) duration = 1231231234;
            this.duration = duration;
        }

        public virtual bool Finished => timer >= duration;

        public virtual void OnAdd()
        {
            passiveEffect?.InitMyself(owner);
        }

        public virtual void OnEnd()
        {
            passiveEffect?.TearDown(owner);
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
                    owner.ApplyHeal(Mathf.RoundToInt(hpChangeSec));
                else
                    owner.ApplyDamage(Mathf.RoundToInt(-hpChangeSec), 0, Vector2.zero);
            }

            if (mpChangeSec != 0)
                if (mpChangeSec > 0)
                    owner.ApplyHealMp(Mathf.RoundToInt(mpChangeSec));
            if (cpChangeSec != 0)
                if (cpChangeSec > 0)
                    owner.ApplyHealCp(Mathf.RoundToInt(cpChangeSec));
        }
    }
}