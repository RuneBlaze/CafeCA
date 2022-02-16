using UnityEngine;

namespace Cafeo.Castable
{
    public class MeleeItem : UsableItem
    {
        public float Radius = 0.2f;
        public float Distance = 2f;
        public MeleeType meleeType;
        private Collider2D[] _results = new Collider2D[10];

        public enum MeleeType
        {
            Stab, // only a point
            Thrust, // spear type
            Hammer, // circle type
        }

        public MeleeItem(float radius, float distance)
        {
            Radius = radius;
            Distance = distance;
            hitAllies = false;
            hitEnemies = true;
            meleeType = MeleeType.Stab;
        }
        
        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            var aimDirection = user.CalcAimDirection(this);
            var targetCoord = (Vector2) user.transform.position + aimDirection * Distance;
            switch (meleeType)
            {
                case MeleeType.Stab:
                    int cnt = Physics2D.OverlapCircleNonAlloc(targetCoord, Radius, _results, targetLayerMask);
                    for (int i = 0; i < cnt; i++)
                    {
                        var vessel = _results[i].GetComponent<BattleVessel>();
                        vessel.ApplyDamage(1, 0.5f, (Vector2) vessel.transform.position - targetCoord);
                    }
                    break;
                case MeleeType.Hammer:
                    var hammerType = new ProjectileType
                    {
                        shape = new ProjectileType.CircleShape(1f),
                        maxSize = Radius,
                    };
                    RogueManager.Instance.CreateProjectiles(hammerType, user, targetCoord, Vector2.zero);
                    break;
                case MeleeType.Thrust:
                    var thrustType = new ProjectileType
                    {
                        shape = new ProjectileType.RectShape(0.2f, 0.8f),
                        timeLimit = 0.2f,
                        speed = Distance / 0.4f,
                        initialFacing = aimDirection,
                    };
                    RogueManager.Instance.CreateProjectiles(thrustType, user, targetCoord - aimDirection/2, aimDirection);
                    break;
            }
        }
    }
}