using Cafeo.Castable;
using UnityEngine;
using static System.Single;

namespace Cafeo.Utility
{
    public class UtilityType
    {
        public float multiplier = 1;
        public float lastTriedUsing = NegativeInfinity;
        
        public float timeCycleInfluence = 150;
        // cool down of 2 seconds, utility goes from -timeCycleInfluence to timeCycleInfluence
        public float timeCycle = -1; // -1 means no time cycle

        public virtual float CalcBaseUtility(BattleVessel user, UsableItem usableItem, UtilityEnv environment)
        {
            return 0;
        }
        public float CalcUtility(BattleVessel user, UsableItem usableItem, UtilityEnv environment)
        {
            var penaltyMult = 1f;
            if (environment.justUsed == usableItem)
            {
                penaltyMult = 1 - environment.justUsedPenalty;
            }
            var baseUtility = CalcBaseUtility(user, usableItem, environment);
            if (timeCycle <= 0)
            {
                return baseUtility * multiplier * penaltyMult;
            }
            else
            {
                var curTime = Time.time;
                var timeDiff = Mathf.Clamp(curTime - lastTriedUsing, 0, 2 * timeCycle);
                var timeDiffRatio = timeDiff / (2 * timeCycle);
                Debug.Log($"timeDiffRatio: {timeDiffRatio}");
                var utility = (baseUtility + (timeDiffRatio - 0.5f) / 0.5f * timeCycleInfluence) * multiplier;
                return utility * penaltyMult;
            }
        }

        public void OnUse(BattleVessel user)
        {
            // lastTriedUsing = Time.time;
        }
        
        public virtual void OnTryUsing(BattleVessel user)
        {
            lastTriedUsing = Time.time;
        }

        public UtilityType SetMultiplier(float v)
        {
            multiplier = v;
            return this;
        }

        public static UtilityType operator +(UtilityType a, UtilityType b)
        {
            return new AddUtilityType(a, b);
        }
        
        public static UtilityType operator *(UtilityType a, float v)
        {
            return new IdUtilityType(a).SetMultiplier(v);
        }

        public class IdUtilityType : UtilityType
        {
            private UtilityType unit;
            public IdUtilityType(UtilityType unit)
            {
                this.unit = unit;
            }
            public override float CalcBaseUtility(BattleVessel user, UsableItem usableItem, UtilityEnv environment)
            {
                return unit.CalcBaseUtility(user, usableItem, environment);
            }
        }

        public class ConstantUtilityType : UtilityType
        {
            private float value;
            public ConstantUtilityType(float value)
            {
                this.value = value;
            }
            public override float CalcBaseUtility(BattleVessel user, UsableItem usableItem, UtilityEnv environment)
            {
                return value;
            }
        }
        
        public static explicit operator UtilityType(float v)
        {
            return new ConstantUtilityType(v);
        }

        public class AddUtilityType : UtilityType
        {
            private UtilityType lhs;
            private UtilityType rhs;

            public AddUtilityType(UtilityType lhs, UtilityType rhs)
            {
                this.lhs = lhs;
                this.rhs = rhs;
            }

            public override void OnTryUsing(BattleVessel user)
            {
                base.OnTryUsing(user);
                lhs.OnTryUsing(user);
                rhs.OnTryUsing(user);
            }

            public override float CalcBaseUtility(BattleVessel user, UsableItem usableItem, UtilityEnv environment)
            {
                return lhs.CalcUtility(user, usableItem, environment) + rhs.CalcUtility(user, usableItem, environment);
            }
        }

        public class SingleEnemyInRange : UtilityType
        {
            public float range;
            public float tol;
            public SingleEnemyInRange(float range, float tol = 60)
            {
                this.range = range;
                this.tol = tol;
            }

            public override float CalcBaseUtility(BattleVessel user, UsableItem usableItem, UtilityEnv environment)
            {
                
                var v1 = environment.distanceToEnemyTarget < range;
                var targetEnemy = user.Brain.RetrieveTargetEnemy();
                if (targetEnemy == null) return -100;
                var v2 = user.IsFacing(targetEnemy, tol);
                if (v1 && v2)
                {
                    return 100;
                }
                else
                {
                    return -10;
                }
            }
        }

        public class SingleEnemyInDirection : UtilityType
        {
            public override float CalcBaseUtility(BattleVessel user, UsableItem usableItem, UtilityEnv environment)
            {
                var targetEnemy = user.Brain.RetrieveTargetEnemy();
                if (targetEnemy == null) return -100;
                return environment.CountForwardEnemies(10) > 0 ? 100 : -10;
            }
        }

        public class DashUtility : UtilityType
        {
            public override float CalcBaseUtility(BattleVessel user, UsableItem usableItem, UtilityEnv environment)
            {
                return user.CanDash ? 100 : -10;
            }
        }

        public class Disregarding : UtilityType
        {
            public override float CalcBaseUtility(BattleVessel user, UsableItem usableItem, UtilityEnv environment)
            {
                return 100;
            }
        }

        public class Cooldown : UtilityType
        {
            public Cooldown(float timeCycle)
            {
                this.timeCycle = timeCycle;
            }
        }

        public class PenalizeDanger : UtilityType
        {
            public override float CalcBaseUtility(BattleVessel user, UsableItem usableItem, UtilityEnv environment)
            {
                return environment.corneringEnemies > 0 ? -200 : 0;
            }
        }
    }
}