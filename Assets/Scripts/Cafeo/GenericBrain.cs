using UnityEngine;

namespace Cafeo
{
    [RequireComponent(typeof(BattleVessel))]
    public abstract class GenericBrain : MonoBehaviour
    {
        public abstract BattleVessel Vessel { get; set; }
        public AgentSoul Soul => Vessel.soul;
        public abstract void DecideAction();
    }
}