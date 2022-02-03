namespace Cafeo.Castable
{
    public class RangedItem : UsableItem
    {
        public ProjectileType ProjectileType;
        public int Shots = 1;
        public int Fan = 0;
        public float Duration = 0;

        public RangedItem(ProjectileType projectileType)
        {
            ProjectileType = projectileType;
        }

        public RangedItem()
        {
            ProjectileType = new ProjectileType
            {
                Shape = new ProjectileType.CircleShape()
            };
        }

        public override void OnUse(BattleVessel user)
        {
            base.OnUse(user);
            if (Duration == 0)
            {
                // we are doing instant use ranged items
                Scene.CreateProjectiles(ProjectileType, user, 
                    user.CalcArrowSpawnLoc(this),
                    user.CalcAimDirection(this));
            }
        }
    }
}