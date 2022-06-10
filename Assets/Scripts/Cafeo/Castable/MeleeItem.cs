using System.Collections.Generic;
using Cafeo.Utility;
using UnityEngine;
using static System.Single;
using Random = UnityEngine.Random;

namespace Cafeo.Castable
{
    public class MeleeItem : UsableItem
    {
        public float radius = 0.2f;
        public float distance = 2f;
        public float bodyThrust = 32f;
        public MeleeType meleeType;
        private Collider2D[] _results = new Collider2D[10];
        private float lastUse = NegativeInfinity;
        private bool coin;

        public enum MeleeType
        {
            Stab, // only a point
            Thrust, // spear type
            Hammer, // circle type
            Scythe, // swing type, cycle 3
            GreatSword,
            BroadSword,
            BodyRush,
        }

        public float EffectiveRange => 0.8f;
        public float EffectiveDegrees => 60f;

        public MeleeItem(float radius, float distance)
        {
            this.radius = radius;
            this.distance = distance;
            hitAllies = false;
            hitEnemies = true;
            meleeType = MeleeType.Stab;
            stopOnUse = false;
            powerType = PowerType.Physical;
            damageType = DamageType.HpDamage;
            utilityType = new UtilityType.SingleEnemyInRange(1f);
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
            var targetCoord = (Vector2) user.transform.position + aimDirection * distance;
            if (Time.time - lastUse > active + startUp + recovery + 0.25f)
            {
                orientation = 0;
                coin = Random.Range(0, 2) == 0;
            }
            lastUse = Time.time;
            var coinI = (coin ? 0 : 1);
            Projectile proj;
            switch (meleeType)
            {
                case MeleeType.Stab:
                    int cnt = Physics2D.OverlapCircleNonAlloc(targetCoord, radius, _results, targetLayerMask);
                    for (int i = 0; i < cnt; i++)
                    {
                        var vessel = _results[i].GetComponent<BattleVessel>();
                        ApplyEffect(user, vessel, user.transform.position, null);
                    }
                    break;
                case MeleeType.Hammer:
                    var hammerType = new ProjectileType
                    {
                        shape = new ProjectileType.CircleShape(1f),
                        maxSize = radius,
                    };
                    CreateMeleeProjectile(hammerType, user, targetCoord, Vector2.zero);
                    break;
                case MeleeType.Thrust:
                    var thrustType = new ProjectileType
                    {
                        shape = new ProjectileType.RectShape(0.2f, 0.8f),
                        timeLimit = 0.2f,
                        speed = distance / 0.4f,
                        initialFacing = aimDirection,
                        collidable = false,
                        kineticBody = false,
                    };
                    proj = CreateMeleeProjectile(thrustType, user, targetCoord - aimDirection/2, aimDirection);
                    proj.RegisterMeleeOwner(this);
                    proj.MoveUnder(user);
                    break;
                case MeleeType.Scythe:
                    var scytheType = new ProjectileType
                    {
                        shape = new ProjectileType.ScytheShape(),
                        rotate = new ProjectileType.RotateType(orientation % 2 == coinI, 120, 0.5f),
                        speed = 0,
                        initialFacing = aimDirection,
                        kineticBody = true,
                    };
                    proj = CreateMeleeProjectile(scytheType, user, targetCoord - aimDirection/2, aimDirection);
                    proj.RegisterMeleeOwner(this);
                    proj.MoveUnder(user);
                    break;
                case MeleeType.GreatSword:
                    var greatSwordType = new ProjectileType
                    {
                        shape = new ProjectileType.GreatSwordShape(),
                        rotate = new ProjectileType.RotateType(orientation % 2 == coinI, 120, 0.5f),
                        speed = 0,
                        initialFacing = aimDirection,
                        kineticBody = true,
                    };
                    proj = CreateMeleeProjectile(greatSwordType, user, targetCoord - aimDirection/2, aimDirection);
                    proj.RegisterMeleeOwner(this);
                    proj.MoveUnder(user);
                    break;
                case MeleeType.BroadSword:
                    var broadSwordType = new ProjectileType
                    {
                        shape = new ProjectileType.RectShape(0.2f, 1.0f, false),
                        rotate = new ProjectileType.RotateType(orientation % 2 == coinI, 150, 0.3f),
                        speed = 0,
                        initialFacing = aimDirection,
                        kineticBody = true,
                    };
                    proj = CreateMeleeProjectile(broadSwordType, user, targetCoord - aimDirection/2, aimDirection);
                    proj.MoveUnder(user);
                    proj.RegisterMeleeOwner(this);
                    break;
                case MeleeType.BodyRush:
                    var bodyRushType = new ProjectileType
                    {
                        shape = new ProjectileType.SquareShape(user.soul.HeightScore * 1.2f),
                        followOwner = true,
                        timeLimit = active,
                    };
                    proj = CreateMeleeProjectile(bodyRushType, user, user.transform.position, Vector2.zero);
                    proj.RegisterMeleeOwner(this);
                    proj.MoveUnder(user);
                    // Debug.Log(aimDirection.normalized);
                    user.AddForce(aimDirection.normalized * bodyThrust);
                    break;
            }
        }

        private Projectile CreateMeleeProjectile(ProjectileType type, BattleVessel user, Vector2 spawnPos, Vector2 dir)
        {
            var res = RogueManager.Instance.CreateProjectile(type, user, spawnPos, dir);
            res.onHit.AddListener(it =>
            {
                ApplyEffect(user, it, user.transform.position, null);
            });
            return res;
        }

        public override void ApplyEffect(BattleVessel user, BattleVessel target, Vector2 hitSource,Projectile hitProj)
        {
            base.ApplyEffect(user, target, hitSource,hitProj);
            // var dmg = Scene.CalculateDamageMelee(user, target, this, false);
            var knockBackDir = (Vector2)target.transform.position - hitSource;
            ApplyCalculatedDamage(user, target, hitStun,knockBackDir.normalized * 70f * knockbackPower);
        }

        public override bool IsGonnaHit(BattleVessel user, BattleVessel target)
        {
            return target.BodyDistance(user) < EffectiveRange && user.IsFacing(target, EffectiveDegrees);
        }
    }
}