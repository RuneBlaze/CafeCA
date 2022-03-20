using System;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Cafeo.Castable
{
    public class TossItem : UsableItem
    {
        public float radius = 2f;
        public float maxDistance = 4;
        public bool alwaysSplash;

        public TossItem()
        {
            stopOnUse = true;
            damageType = DamageType.HpRecovery;
        }

        public void Update()
        {
            
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            if (maxDistance <= 0 && coroutineOnStart != null) return;
            var aimTarget = user.CalcAimTarget(this);
            if (alwaysSplash && aimTarget == null)
            {
                aimTarget = null;
            }
            // if (alwaysSplash && maxDistance <= 0)
            // {
            //     aimTarget = null;
            // }
            if (aimTarget == null)
            {
                // self use
                // ApplyEffect(user, user);
                CreateSplashProjectile(user, user.transform);
            }
            else
            {
                // we actually toss stuff now
                // ApplyEffect(user, aimTarget);
                var type = new ProjectileType
                {
                    shape = new ProjectileType.CircleShape { radius = 0.2f },
                    pierce = 1,
                    collidable = true,
                    speed = 8f,
                    hitAllies = hitAllies,
                    hitEnemies = hitEnemies,
                };
                var dir = (aimTarget.transform.position - user.transform.position).normalized * user.Radius;
                var proj = Scene.CreateProjectile(type, user, user.transform.position + dir * 1f , dir);
                proj.beforeDestroy.AddListener(() => { CreateSplashProjectile(user, proj.transform); });
            }
        }

        private void CreateSplashProjectile(BattleVessel user, Transform origin)
        {
            var splash = new ProjectileType
            {
                shape = new ProjectileType.CircleShape { radius = radius },
                collidable = false,
                speed = 0f,
                hitAllies = hitAllies,
                hitEnemies = hitEnemies,
                deltaSize = -1.2f,
            };
            var splashProj = Scene.CreateProjectile(splash, user, origin.position, Vector2.down);
            splashProj.onHit.AddListener(target => { ApplyEffect(user, target, splashProj.transform.position, splashProj); });
        }

        public override void ApplyEffect(BattleVessel user, BattleVessel target, Vector2 hitSource, Projectile hitProj)
        {
            base.ApplyEffect(user, target, hitSource, hitProj);
            Vector2 towardsTarget = (Vector2) target.transform.position - hitSource;
            ApplyCalculatedDamage(user, target, hitStun, towardsTarget * 0.5f);
            // target.ApplyHeal(10);
        }
        // public void ApplyEffect(BattleVessel user, BattleVessel target)
        // {
        //     target.ApplyHeal(10);
        // }
    }
}