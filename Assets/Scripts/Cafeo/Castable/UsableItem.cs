using System;

namespace Cafeo.Castable
{
    [Serializable]
    public class UsableItem
    {
        public string name;
        public float startUp;
        public float active;
        public float recovery = 0.5f;

        public float cd;
        public bool forceCasting;
        public bool startUpMovable;
        public float baseDamage;
        
        public RogueManager Scene => RogueManager.Instance;

        public virtual void OnUse(BattleVessel user)
        {
            
        }

        public virtual void DuringActive(BattleVessel user, float timer)
        {
            
        }
    }
}