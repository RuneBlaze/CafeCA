using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

            public CircleShape(float radius = 0.1f)
            {
                this.radius = radius;
            }

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

        public class RectShape : ProjectileShape
        {
            public float width = 0.2f;
            public float height = 0.6f;

            public RectShape(float width, float height)
            {
                this.width = width;
                this.height = height;
            }

            public Vector3[] vertices;

            public override Collider2D CreateCollider(GameObject gameObject)
            {
                vertices = new Vector3[4];
                var component = gameObject.AddComponent<BoxCollider2D>();
                component.size = new Vector2(width, height);
                return component;
            }
        }

        public class CustomShape : ProjectileShape
        {
            public GameObject prefab;
            public Vector3[] vertices;

            public CustomShape(GameObject prefab)
            {
                this.prefab = prefab;
            }

            public CustomShape(string path)
            {
                prefab = Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
            }

            public override Collider2D CreateCollider(GameObject gameObject)
            {
                var src = prefab.GetComponent<PolygonCollider2D>();
                var dst = gameObject.AddComponent<PolygonCollider2D>();
                dst.points = src.points;
                dst.offset = src.offset;
                vertices = new Vector3[dst.points.Length];
                return dst;
            }
        }

        public ProjectileShape shape;
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
        public float deltaSize;
        public float maxSize;
        public float density = 32f;

        public Vector2 initialFacing = Vector2.zero;
        // public float timeLimit;

        public float boomerangStrength;
    }
}