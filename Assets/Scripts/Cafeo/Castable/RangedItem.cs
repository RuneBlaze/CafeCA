using System.Collections;
using Cafeo.Utils;
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

        private void ShootOnce(BattleVessel user)
        {
            projectileType.hitAllies = hitAllies;
            projectileType.hitEnemies = hitEnemies;
            // we are doing instant use ranged items
            if (fan == 0 || spread == 0)
            {
                Scene.CreateProjectiles(projectileType, user, 
                    user.CalcArrowSpawnLoc(this),
                    user.CalcAimDirection(this));
            }
            else
            {
                Scene.CreateFanProjectiles(projectileType, fan, spread, user,
                    user.CalcArrowSpawnLoc(this),
                    user.CalcAimDirection(this));
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
            if (shots > 1)
            {
                Assert.IsTrue(duration > 0);
                coroutineOnStart = MultiShotLogic(user);
            }
            base.OnUse(user);
            if (duration == 0 && projectileType != null && shots == 1)
            {
                ShootOnce(user);
            }
        }
    }
}