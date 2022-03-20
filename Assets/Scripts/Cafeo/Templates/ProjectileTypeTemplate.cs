using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class ProjectileTypeTemplate : WithDisplayName, ITemplate<ProjectileType>
    {
        [BoxGroup("Basic Info", centerLabel: true)]
        public ProjectileType.PrimitiveShapes shapeType;

        [BoxGroup("Basic Info", centerLabel: true)]
        public Vector4 shapeUserData;

        [BoxGroup("Basic Info", centerLabel: true)]
        public int pierce;
        [BoxGroup("Basic Info", centerLabel: true)]
        public int bounce = -1;
        [BoxGroup("Basic Info", centerLabel: true)]
        public SkillTemplate.TemplateHitType hitType;
        [BoxGroup("Basic Info", centerLabel: true)]
        public bool collidable;
        [BoxGroup("Basic Info", centerLabel: true)]
        public float timeLimit = 30f;
        [BoxGroup("Speed", centerLabel: true)]
        public float speed = 2f;
        [BoxGroup("Speed", centerLabel: true)]
        public float acceleration;
        [BoxGroup("Speed", centerLabel: true)]
        public float maxSpeed;
        [BoxGroup("Size Change", centerLabel: true)]
        public float deltaSize;
        [BoxGroup("Size Change", centerLabel: true)]
        public float maxSize;
        [BoxGroup("Physics Properties", centerLabel: true)]
        public float density = 32f;
        [BoxGroup("Physics Properties", centerLabel: true)]
        public bool kineticBody = false;
        [BoxGroup("Physics Properties", centerLabel: true)]
        public bool bullet;
        [BoxGroup("Physics Properties", centerLabel: true)]
        public float bounciness = 0.8f;
        [BoxGroup("Misc", centerLabel: true)]
        public float initialSpin = 0;
        [BoxGroup("Misc", centerLabel: true)]
        public float boomerang = 0;
        [BoxGroup("Misc", centerLabel: true)]
        public bool followOwner;
        [BoxGroup("Misc", centerLabel: true)]
        public float homingRadius = 2f;
        [BoxGroup("Misc", centerLabel: true)]
        public float homingStrength = 0;

        public ProjectileType Generate()
        {
            var projectileType = new ProjectileType
            {
                shape = ProjectileType.CreatePrimitiveShape(shapeType, shapeUserData),
                pierce = pierce,
                bounce = bounce,
                hitAllies = hitType is SkillTemplate.TemplateHitType.HitAllies or SkillTemplate.TemplateHitType.HitBoth,
                hitEnemies = hitType is SkillTemplate.TemplateHitType.HitEnemies or SkillTemplate.TemplateHitType.HitBoth,
                collidable = collidable,
                timeLimit = timeLimit,
                speed = speed,
                acceleration = acceleration,
                maxSpeed = maxSpeed,
                deltaSize = deltaSize,
                maxSize = maxSize,
                density = density,
                kineticBody = kineticBody,
                bullet = bullet,
                bounciness = bounciness,
                initialSpin = initialSpin,
                boomerang = boomerang,
                followOwner = followOwner,
                homingRadius = homingRadius,
                homingStrength = homingStrength
            };
            return projectileType;
        }
    }
}