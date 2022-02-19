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
        
        public static void RotateTowards(Transform transform, Vector2 target, float speed)
        {
            Vector2 dir = target - (Vector2)transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.AngleAxis(angle, Vector3.forward), speed);
        }
    }
}