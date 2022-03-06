namespace Cafeo
{
    // 宝物
    public class Treasure
    {
        public BattleVessel owner;
        public string displayName;

        public bool LeaderEquipped => true;
        public RogueManager Scene => RogueManager.Instance;

        public virtual void OnEquip()
        {
            
        }

        public virtual void OnUnequip()
        {
            
        }
    }
}