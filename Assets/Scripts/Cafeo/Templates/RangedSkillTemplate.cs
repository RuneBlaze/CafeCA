using Cafeo.Castable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class RangedSkillTemplate : SkillTemplate
    {
        [BoxGroup("Ranged Specification", centerLabel: true)]
        public int shots = 1;

        [BoxGroup("Ranged Specification", centerLabel: true)]
        public int fan;

        [BoxGroup("Ranged Specification", centerLabel: true)] [ProgressBar(0, 360)]
        public int spread;

        [BoxGroup("Ranged Specification", centerLabel: true)]
        public float duration;

        [BoxGroup("Ranged Specification", centerLabel: true)]
        public float instability;

        [BoxGroup("Ranged Specification", centerLabel: true)]
        public bool withPrimaryShot;

        [BoxGroup("Bullet Override", centerLabel: true)]
        public int pierce = -1;

        [BoxGroup("Bullet Override", centerLabel: true)]
        public int bounce = -1;

        [BoxGroup("Bullet Override", centerLabel: true)]
        public float speed = -1;

        [BoxGroup("Bullet Override", centerLabel: true)]
        public float acceleration = -1;

        [BoxGroup("Bullet Specifications", centerLabel: true)]
        public ProjectileTypeTemplate simpleProjectile;

        [BoxGroup("Bullet Specifications", centerLabel: true)]
        public GameObject projectilePrefab;

        private void Reset()
        {
            hitType = TemplateHitType.HitEnemies;
        }

        public override UsableItem Generate()
        {
            var item = new RangedItem();
            CopyBaseParameters(item);
            item.shots = shots;
            item.fan = fan;
            item.spread = spread;
            item.duration = duration;
            item.instability = instability;
            item.withPrimaryShot = withPrimaryShot;
            // item.active = 0.3f;
            // item.recovery = 0.05f;
            if (simpleProjectile != null)
            {
                item.projectileType = simpleProjectile.Generate();
                // Debug.Log(item.projectileType.shape);
                if (pierce != -1) item.projectileType.pierce = pierce;
                if (bounce != -1) item.projectileType.bounce = bounce;
                if (speed >= 0) item.projectileType.speed = speed;
                if (acceleration >= 0) item.projectileType.acceleration = acceleration;
            }

            return item;
        }
    }
}