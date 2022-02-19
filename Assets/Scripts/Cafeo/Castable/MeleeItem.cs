using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.Castable
{
    public class MeleeItem : UsableItem
    {
        public float Radius = 0.2f;
        public float Distance = 2f;
        public MeleeType meleeType;
        private Collider2D[] _results = new Collider2D[10];

        public enum MeleeType
        {
            Stab, // only a point
            Thrust, // spear type
            Hammer, // circle type
            Scythe, // swing type, cycle 3
            GreatSword,
            BroadSword,
        }

        public MeleeItem(float radius, float distance)
        {
            Radius = radius;
            Distance = distance;
            hitAllies = false;
            hitEnemies = true;
            meleeType = MeleeType.Stab;
        }

        public override void Setup(BattleVessel user)
        {
            base.Setup(user);
            if (meleeType is MeleeType.Scythe or MeleeType.GreatSword or MeleeType.BroadSword)
            {
                orbit = 2;
            }
        }

        public override void OnCounter(BattleVessel user)
        {
            base.OnCounter(user);
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            var aimDirection = user.CalcAimDirection(this);
            var targetCoord = (Vector2) user.transform.position + aimDirection * Distance;
            Projectile proj;
            switch (meleeType)
            {
                case MeleeType.Stab:
                    int cnt = Physics2D.OverlapCircleNonAlloc(targetCoord, Radius, _results, targetLayerMask);
                    for (int i = 0; i < cnt; i++)
                    {
                        var vessel = _results[i].GetComponent<BattleVessel>();
                        vessel.ApplyDamage(1, 0.5f, (Vector2) vessel.transform.position - targetCoord);
                    }
                    break;
                case MeleeType.Hammer:
                    var hammerType = new ProjectileType
                    {
                        shape = new ProjectileType.CircleShape(1f),
                        maxSize = Radius,
                    };
                    RogueManager.Instance.CreateProjectile(hammerType, user, targetCoord, Vector2.zero);
                    break;
                case MeleeType.Thrust:
                    var thrustType = new ProjectileType
                    {
                        shape = new ProjectileType.RectShape(0.2f, 0.8f),
                        timeLimit = 0.2f,
                        speed = Distance / 0.4f,
                        initialFacing = aimDirection,
                        collidable = false,
                        kineticBody = false,
                    };
                    proj = RogueManager.Instance.CreateProjectile(thrustType, user, targetCoord - aimDirection/2, aimDirection);
                    proj.RegisterMeleeOwner(this);
                    proj.transform.parent = user.transform;
                    break;
                case MeleeType.Scythe:
                    var scytheType = new ProjectileType
                    {
                        shape = new ProjectileType.ScytheShape(),
                        rotate = new ProjectileType.RotateType(orientation % 2 == 0, 120, 0.5f),
                        speed = 0,
                        initialFacing = aimDirection,
                        kineticBody = true,
                    };
                    proj = RogueManager.Instance.CreateProjectile(scytheType, user, targetCoord - aimDirection/2, aimDirection);
                    proj.RegisterMeleeOwner(this);
                    proj.transform.parent = user.transform;
                    
                    break;
                case MeleeType.GreatSword:
                    var greatSwordType = new ProjectileType
                    {
                        shape = new ProjectileType.GreatSwordShape(),
                        rotate = new ProjectileType.RotateType(orientation % 2 == 0, 120, 0.5f),
                        speed = 0,
                        initialFacing = aimDirection,
                        kineticBody = true,
                    };
                    proj = RogueManager.Instance.CreateProjectile(greatSwordType, user, targetCoord - aimDirection/2, aimDirection);
                    proj.RegisterMeleeOwner(this);
                    proj.transform.parent = user.transform;
                    break;
                case MeleeType.BroadSword:
                    var broadSwordType = new ProjectileType
                    {
                        shape = new ProjectileType.RectShape(0.2f, 1.0f, false),
                        rotate = new ProjectileType.RotateType(orientation % 2 == 0, 150, 0.3f),
                        speed = 0,
                        initialFacing = aimDirection,
                        kineticBody = true,
                    };
                    proj = RogueManager.Instance.CreateProjectile(broadSwordType, user, targetCoord - aimDirection/2, aimDirection);
                    proj.transform.parent = user.transform;
                    proj.RegisterMeleeOwner(this);
                    break;
            }
            user.StopMoving();
        }
    }
}