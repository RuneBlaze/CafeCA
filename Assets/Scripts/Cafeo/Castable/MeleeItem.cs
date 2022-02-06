using UnityEngine;

namespace Cafeo.Castable
{
    public class MeleeItem : UsableItem
    {
        public float Radius = 0.2f;
        public float Distance = 2f;
        private Collider2D[] _results = new Collider2D[10];

        public MeleeItem(float radius, float distance)
        {
            Radius = radius;
            Distance = distance;
            hitAllies = false;
            hitEnemies = true;
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            var targetCoord = (Vector2) user.transform.position + user.CalcAimDirection(this) * Distance;
            int cnt = Physics2D.OverlapCircleNonAlloc(targetCoord, Radius, _results, targetLayerMask);
            for (int i = 0; i < cnt; i++)
            {
                var vessel = _results[i].GetComponent<BattleVessel>();
                vessel.ApplyDamage(1, 0.5f, (Vector2) vessel.transform.position - targetCoord);
            }
        }
    }
}