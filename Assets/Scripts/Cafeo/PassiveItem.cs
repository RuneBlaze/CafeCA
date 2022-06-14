namespace Cafeo
{
    public class PassiveItem : AbstractItem
    {
        public HitEffects initEffects;

        public void InitMyself(BattleVessel owner)
        {
            var user = owner;
            if (owner.IsAlly)
                foreach (var ally in RogueManager.Instance.Allies())
                    initEffects.Apply(owner, ally);
            else
                initEffects.Apply(owner, owner);
        }

        public void TearDown(BattleVessel owner)
        {
        }
    }
}