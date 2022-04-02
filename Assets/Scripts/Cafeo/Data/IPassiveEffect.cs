namespace Cafeo.Data
{
    public interface IPassiveEffect
    {
        public void InitMyself(BattleVessel owner);
        public void TearDown(BattleVessel owner);
        public void EveryTick(BattleVessel owner);
        public void EverySec(BattleVessel owner);
    }
}