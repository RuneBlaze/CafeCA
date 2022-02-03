namespace Cafeo.Castable
{
    public class MeleeItem : UsableItem
    {
        public float Radius = 0.2f;
        public float Distance = 2f;

        public MeleeItem(float radius, float distance)
        {
            Radius = radius;
            Distance = distance;
        }
    }
}