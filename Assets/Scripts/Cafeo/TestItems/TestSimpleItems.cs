using Cafeo;
using Cafeo.Castable;

namespace DefaultNamespace
{
    public class TestSimpleItems
    {
        public static RangedItem MagicDart()
        {
            var rangedItem = new RangedItem
            {
                ProjectileType = new ProjectileType
                {
                    collidable = false,
                    speed = 20f,
                    hitEnemies = true,
                    hitAllies = false,
                    pierce = 1
                }
            };
            return rangedItem;
        }
    }
}