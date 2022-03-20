using System;
using System.Runtime.InteropServices;

namespace Cafeo
{
    public class PassiveItem : AbstractItem
    {
        public HitEffects initEffects;
        public HitEffects.BuffExpr DetermineEffect()
        {
            throw new NotImplementedException();
        }

        public virtual void InitMyself(BattleVessel owner)
        {
            initEffects.Apply(owner, owner);
        }

        public virtual void TearDown(BattleVessel owner)
        {
            initEffects.TearDown(owner);
        }

        public virtual void EveryTick()
        {
            
        }

        public virtual void EverySec()
        {
            
        }
    }
}