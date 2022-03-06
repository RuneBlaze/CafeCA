using UnityEngine;

namespace Cafeo.Utils
{
    public static class VectorUtils
    {
        public static Vector2 RotateVector(Vector2 vec, float degs)
        {
            float rads = degs * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rads);
            float cos = Mathf.Cos(rads);
            return new Vector2(vec.x * cos - vec.y * sin, vec.x * sin + vec.y * cos);
        }

        public static float DegreesBetween(Vector2 lhs, Vector2 rhs)
        {
            return Mathf.DeltaAngle(AngleOf(lhs), AngleOf(rhs));
        }
        
        public static float AngleOf(Vector2 vec)
        {
            return Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        }

        public static Vector2 RotateVectorTowards(Vector2 src, Vector2 dst, float t)
        {
            return Vector3.Slerp(src, dst, t).normalized * src.magnitude;
        }
        
        public static void RotateTowards(Transform transform, Vector2 target, float speed)
        {
            Vector2 dir = target - (Vector2)transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.AngleAxis(angle, Vector3.forward), speed);
        }
        
        public static void RotateTowards(Rigidbody2D body, Vector2 target, float speed)
        {
            Vector2 dir = target - (Vector2)body.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            body.velocity = RotateVectorTowards(body.velocity, dir, speed);        }

        public static float Sgn(float x)
        {
            if (Mathf.Abs(x) < 0.01f)
            {
                return 0;
            }

            return Mathf.Sign(x);
        }
        
        public static LayerMask GetCollisionMaskOf(GameObject go)
        {
            int myLayer = go.layer;
            int layerMask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!Physics2D.GetIgnoreLayerCollision(myLayer, i))
                {
                    layerMask |= 1 << i;
                }
            }
            return layerMask;
        }
    }
}