using System;
using UnityEngine;

namespace Cafeo
{
    [Serializable]
    public class ProjectileType
    {
        public abstract class ProjectileShape
        {
            public abstract Collider2D CreateCollider(GameObject gameObject);
        }
        
        public class CircleShape : ProjectileShape
        {
            public float radius = 0.1f;
            
            public override Collider2D CreateCollider(GameObject gameObject)
            {
                var component = gameObject.AddComponent<CircleCollider2D>();
                component.radius = radius;
                return component;
            }
        }
        
        public class SquareShape : ProjectileShape
        {
            public float size = 0.2f;
            
            public override Collider2D CreateCollider(GameObject gameObject)
            {
                var component = gameObject.AddComponent<BoxCollider2D>();
                component.size = new Vector2(size, size);
                return component;
            }
        }

        public ProjectileShape Shape;
        public int pierce;
        public int bounce;
        public bool hitAllies;
        public bool hitEnemies;
        public bool collidable;
        public float timeLimit = 30f;
        public float speed = 2f;
        public float drag;
        public float forceMultiplier;
        public float baseDamage;
    }
}