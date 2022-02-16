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

        public void Update()
        {
            
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            var aimTarget = user.CalcAimTarget(this);
            if (alwaysSplash && aimTarget == null)
            {
                aimTarget = user;
            }
            if (aimTarget == null)
            {
                // self use
                ApplyEffect(user, user);
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
                    speed = 20f,
                    hitAllies = hitAllies,
                    hitEnemies = hitEnemies,
                    baseDamage = -1
                };
                var dir = (aimTarget.transform.position - user.transform.position).normalized * user.Radius;
                var proj = Scene.CreateProjectiles(type, user, user.transform.position + dir * 1.5f , dir);
                proj.beforeDestroy.AddListener(() =>
                {
                    var splash = new ProjectileType
                    {
                        shape = new ProjectileType.CircleShape { radius = radius },
                        collidable = false,
                        speed = 0f,
                        hitAllies = hitAllies,
                        hitEnemies = hitEnemies,
                        baseDamage = -1,
                        deltaSize = -0.5f,
                    };
                    Scene.CreateProjectiles(splash, user, proj.transform.position, Vector2.down);
                });
            }
        }

        public void ApplyEffect(BattleVessel user, BattleVessel target)
        {
            target.ApplyHeal(10);
        }
    }
}