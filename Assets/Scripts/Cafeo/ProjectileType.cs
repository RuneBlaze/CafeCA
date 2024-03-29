﻿using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Cafeo
{
    [Serializable]
    public class ProjectileType
    {
        public enum PrimitiveShapes
        {
            Circle,
            Box,
            Rect
        }

        public int pierce;
        public int bounce = -1;
        public bool hitAllies;
        public bool hitEnemies;
        public bool collidable;
        public float timeLimit = 30f;
        public float speed = 2f;
        public float acceleration;
        public float maxSpeed;
        public float deltaSize;
        public float maxSize;
        public float density = 32f;
        public bool kineticBody;
        public float initialSpin;
        public float boomerang;
        public Vector2 initialFacing = Vector2.zero;
        public float bounciness = 0.8f;
        public bool bullet;
        public bool followOwner;
        public float homingRadius = 2f;
        public float homingStrength;
        public RotateType rotate;

        public ProjectileShape shape;

        public static ProjectileShape CreatePrimitiveShape(PrimitiveShapes shapeType, Vector4 userData)
        {
            return shapeType switch
            {
                PrimitiveShapes.Box => new SquareShape(userData[0]),
                PrimitiveShapes.Circle => new CircleShape(userData[0]),
                PrimitiveShapes.Rect => new RectShape(userData[0], userData[1]),
                _ => throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null)
            };
        }

        [Serializable]
        public abstract class ProjectileShape
        {
            public abstract Collider2D CreateCollider(GameObject gameObject);
        }

        [Serializable]
        public class CircleShape : ProjectileShape
        {
            public float radius;

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

        [Serializable]
        public class SquareShape : ProjectileShape
        {
            public float size = 0.2f;

            public SquareShape(float size)
            {
                this.size = size;
            }

            public override Collider2D CreateCollider(GameObject gameObject)
            {
                var component = gameObject.AddComponent<BoxCollider2D>();
                component.size = new Vector2(size, size);
                return component;
            }
        }

        [Serializable]
        public class RectShape : ProjectileShape
        {
            public float width = 0.2f;
            public float height = 0.6f;
            public bool centering;

            public Vector3[] vertices;

            public RectShape(float width, float height, bool centering = true)
            {
                this.width = width;
                this.height = height;
                this.centering = centering;
            }

            public override Collider2D CreateCollider(GameObject gameObject)
            {
                vertices = new Vector3[4];
                var component = gameObject.AddComponent<BoxCollider2D>();
                component.size = new Vector2(width, height);
                if (!centering) component.offset = new Vector2(0, -height / 2 - 0.1f);
                return component;
            }
        }

        [Serializable]
        public class CustomShape : ProjectileShape
        {
            public GameObject prefab;
            public Vector3[] vertices;
            public float scale = 1;

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
                for (var i = 0; i < dst.points.Length; i++) dst.points[i] *= scale;
                dst.offset = src.offset * scale;
                vertices = new Vector3[dst.points.Length];
                return dst;
            }

            public CustomShape Rescale(float x)
            {
                scale = x;
                return this;
            }
        }

        public class ScytheShape : CustomShape
        {
            public ScytheShape() : base("Assets/Data/Shapes/Scythe.prefab")
            {
            }
        }

        public class GreatSwordShape : CustomShape
        {
            public GreatSwordShape() : base("Assets/Data/Shapes/GreatSword.prefab")
            {
            }
        }

        public class PredefinedShape : CustomShape
        {
            public PredefinedShape(string name) : base($"Assets/Data/Shapes/{name}.prefab")
            {
            }
        }

        // swings towards orientation direction across "range" in "speed" seconds
        public class RotateType
        {
            public bool orientation;
            public float range;
            public float speed;

            public RotateType(bool orientation, float range, float speed)
            {
                this.orientation = orientation;
                this.range = range;
                this.speed = speed;
            }
        }
    }
}