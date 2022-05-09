using Cafeo.Castable;

namespace Cafeo.Data
{
    public interface IPassiveEffect
    {
        public void InitMyself(BattleVessel owner);
        public void TearDown(BattleVessel owner);
        public void EveryTick(BattleVessel owner);
        public void EverySec(BattleVessel owner);
        public void InfluenceProjectile(Projectile projectile);
        public void InfluenceSkill(UsableItem item);
        public void OnDamage(BattleVessel owner);
    }
}