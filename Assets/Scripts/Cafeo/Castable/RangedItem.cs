using System.Collections;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo.Castable
{
    public class RangedItem : UsableItem
    {
        public ProjectileType projectileType;
        public int shots = 1;
        public int fan = 0;
        public int spread = 0;
        public float duration = 0;
        public float instability = 0;

        public RangedItem(ProjectileType projectileType)
        {
            this.projectileType = projectileType;
        }

        public RangedItem()
        {
            projectileType = new ProjectileType
            {
                shape = new ProjectileType.CircleShape()
            };
        }

        public override void Setup(BattleVessel user)
        {
            base.Setup(user);
            if (shots > 1)
            {
                Assert.IsTrue(duration > 0);
                coroutineOnStart = MultiShotLogic(user);
            }
        }

        private void ShootOnce(BattleVessel user)
        {
            projectileType.hitAllies = hitAllies;
            projectileType.hitEnemies = hitEnemies;
            // we are doing instant use ranged items
            var arrowSpawnLoc = user.CalcArrowSpawnLoc(this);
            var a = Random.Range(-instability / 2, instability / 2);
            if (instability > 0)
            {
                // arrowSpawnLoc = arrowSpawnLoc.Rotate(a);
            }
            if (fan == 0 || spread == 0)
            {
                Scene.CreateProjectile(projectileType, user, 
                    arrowSpawnLoc,
                    user.CalcAimDirection(this).Rotate(a));
            }
            else
            {
                Scene.CreateFanProjectiles(projectileType, fan, spread, user,
                    arrowSpawnLoc,
                    user.CalcAimDirection(this).Rotate(a));
            }
        }

        private IEnumerator MultiShotLogic(BattleVessel user)
        {
            var interval = duration / (shots+1);
            ShootOnce(user);
            for (int i = 0; i < shots - 1; i++)
            {
                yield return new WaitForRogueSeconds(interval);
                ShootOnce(user);
            }
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            if (duration == 0 && projectileType != null && shots == 1)
            {
                ShootOnce(user);
            }
        }
    }
}