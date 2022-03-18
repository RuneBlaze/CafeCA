using System.Collections;
using Cafeo.Utility;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo.Castable
{
    public class RangedItem : UsableItem
    {
        public ProjectileType projectileType;
        public int shots = 1;
        public int fan = 0;
        public int spread = 0;
        public float duration = 0;
        public float instability = 0;
        public bool withPrimaryShot;

        public RangedItem(ProjectileType projectileType)
        {
            this.projectileType = projectileType;
            stopOnUse = true;
            damageType = DamageType.HpDamage;
            powerType = PowerType.Magic;
            utilityType = new UtilityType.SingleEnemyInRange(15f, 10f);
        }

        public RangedItem() : this(new ProjectileType
        {
            shape = new ProjectileType.CircleShape()
        })
        {
            
        }

        public override void Setup(BattleVessel user)
        {
            base.Setup(user);
            if (shots > 1)
            {
                Assert.IsTrue(duration > 0);
                coroutineOnStart = MultiShotLogic(user);
            }
        }

        private void ShootOnce(BattleVessel user)
        {
            projectileType.hitAllies = hitAllies;
            projectileType.hitEnemies = hitEnemies;
            // we are doing instant use ranged items
            var arrowSpawnLoc = user.CalcArrowSpawnLoc(this);
            var a = Random.Range(-instability / 2, instability / 2);
            if (instability > 0)
            {
                // arrowSpawnLoc = arrowSpawnLoc.Rotate(a);
            }
            if (fan == 0 || spread == 0)
            {
                var proj = Scene.CreateProjectile(projectileType, user,
                    arrowSpawnLoc,
                    user.CalcAimDirection(this).Rotate(a));
                proj.onHit.AddListener(it =>
                {
                    ApplyEffect(user, it, proj.transform.position, proj);
                });
            }
            else
            {
                var projs = Scene.CreateFanProjectiles(projectileType, fan, spread, user,
                    arrowSpawnLoc,
                    user.CalcAimDirection(this).Rotate(a));
                foreach (var proj in projs)
                {
                    proj.onHit.AddListener(it =>
                    {
                        ApplyEffect(user, it, proj.transform.position, proj);
                    });
                }
            }
        }
        
        public override void ApplyEffect(BattleVessel user, BattleVessel target, Vector2 hitSource, Projectile hitProj)
        {
            base.ApplyEffect(user, target, hitSource, hitProj);
            // var dmg = Scene.CalculateDamageRanged(user, target, this, false);
            // var knockBackDir = (Vector2)target.transform.position - hitSource;
            // target.ApplyDamage(dmg, 0.2f, hitProj.Velocity * 5f);
            ApplyCalculatedDamage(user, target, hitStun, hitProj.Velocity * 5f);
        }

        private IEnumerator MultiShotLogic(BattleVessel user)
        {
            var interval = duration / (shots+1);
            ShootOnce(user);
            for (int i = 0; i < shots - 1; i++)
            {
                yield return new WaitForRogueSeconds(interval);
                ShootOnce(user);
            }
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            if (duration == 0 && projectileType != null && shots == 1)
            {
                ShootOnce(user);
            }

            if (withPrimaryShot)
            {
                user.UsePrimaryShot();
            }
            
            // user.StopMoving();
        }
    }
}