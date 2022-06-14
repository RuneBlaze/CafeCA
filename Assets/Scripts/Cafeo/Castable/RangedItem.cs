using System.Collections;
using Cafeo.Utility;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo.Castable
{
    public class RangedItem : UsableItem
    {
        public float duration = 0;
        public float durationMod;
        public int fan = 0;
        public int fanMod;
        public float instability = 0;
        public float instabilityMod;
        public ProjectileType projectileType;
        public int shotMod;

        public int shots = 1;
        public int spread = 0;
        public int spreadMod;
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

        public int EffectiveShots => shots + shotMod;
        public int EffectiveFan => fan + fanMod;
        public int EffectiveSpread => spread + spreadMod;
        public float EffectiveDuration => duration + durationMod;
        public float EffectiveInstability => instability + instabilityMod;

        public override void Setup(BattleVessel user)
        {
            base.Setup(user);
            if (EffectiveShots > 1)
            {
                // Debug.Log(EffectiveDuration);
                Assert.IsTrue(EffectiveDuration > 0);
                coroutineOnStart = MultiShotLogic(user);
            }
        }

        private void ShootOnce(BattleVessel user)
        {
            projectileType.hitAllies = hitAllies;
            projectileType.hitEnemies = hitEnemies;
            // we are doing instant use ranged items
            var arrowSpawnLoc = user.CalcArrowSpawnLoc(this);
            var a = Random.Range(-EffectiveInstability / 2, EffectiveInstability / 2);
            if (EffectiveFan == 0 || EffectiveSpread == 0)
            {
                var proj = Scene.CreateProjectile(projectileType, user,
                    arrowSpawnLoc,
                    user.CalcAimDirection(this).Rotate(a));
                proj.onHit.AddListener(it => { ApplyEffect(user, it, proj.transform.position, proj); });
                foreach (var effect in user.PassiveEffects()) effect.InfluenceProjectile(proj);
            }
            else
            {
                var projs = Scene.CreateFanProjectiles(projectileType, EffectiveFan, EffectiveSpread, user,
                    arrowSpawnLoc,
                    user.CalcAimDirection(this).Rotate(a));
                foreach (var proj in projs)
                    proj.onHit.AddListener(it => { ApplyEffect(user, it, proj.transform.position, proj); });
                foreach (var effect in user.PassiveEffects())
                foreach (var proj in projs)
                    effect.InfluenceProjectile(proj);
            }
        }

        public override void ApplyEffect(BattleVessel user, BattleVessel target, Vector2 hitSource, Projectile hitProj)
        {
            base.ApplyEffect(user, target, hitSource, hitProj);
            // var dmg = Scene.CalculateDamageRanged(user, target, this, false);
            // var knockBackDir = (Vector2)target.transform.position - hitSource;
            // target.ApplyDamage(dmg, 0.2f, hitProj.Velocity * 5f);
            ApplyCalculatedDamage(user, target, hitStun, hitProj.Velocity * 5f * knockbackPower);
        }

        private IEnumerator MultiShotLogic(BattleVessel user)
        {
            var interval = EffectiveDuration / (EffectiveShots + 1);
            ShootOnce(user);
            for (var i = 0; i < EffectiveShots - 1; i++)
            {
                yield return new WaitForRogueSeconds(interval);
                ShootOnce(user);
            }
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            if (EffectiveDuration == 0 && projectileType != null && EffectiveShots == 1) ShootOnce(user);

            if (withPrimaryShot) user.UsePrimaryShot();

            // user.StopMoving();
        }

        public override void Reset()
        {
            base.Reset();
            fanMod = 0;
            spreadMod = 0;
            shotMod = 0;
            durationMod = 0;
            instabilityMod = 0;
        }
    }
}