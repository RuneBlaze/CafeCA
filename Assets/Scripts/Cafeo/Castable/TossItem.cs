using System;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Cafeo.Castable
{
    public class TossItem : UsableItem
    {
        public float radius;
        public float maxDistance;

        public void Update()
        {
            
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            var aimTarget = user.CalcAimTarget(this);
            if (aimTarget == null)
            {
                // self use
                ApplyEffect(user, user);
            }
            else
            {
                // we actually toss stuff now
                // TODO: implement tossing and splashing
                // ApplyEffect(user, aimTarget);
                var type = new ProjectileType
                {
                    Shape = new ProjectileType.CircleShape { radius = 0.2f },
                    pierce = 1,
                    collidable = true,
                    speed = 20f,
                    hitAllies = hitAllies,
                    hitEnemies = hitEnemies,
                    baseDamage = -1
                };
                var dir = (aimTarget.transform.position - user.transform.position).normalized * 0.5f;
                var proj = Scene.CreateProjectiles(type, user, user.transform.position + dir, dir);
                proj.beforeDestroy.AddListener(() =>
                {
                    var splash = new ProjectileType
                    {
                        Shape = new ProjectileType.CircleShape { radius = 2f },
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