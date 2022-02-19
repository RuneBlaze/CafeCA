using UnityEngine;

namespace Cafeo.Utils
{
    public static class Vector2Extension
    {
        public static Vector2 Rotate(this Vector2 v, float degs)
        {
            return VectorUtils.RotateVector(v, degs);
        }
    }
}