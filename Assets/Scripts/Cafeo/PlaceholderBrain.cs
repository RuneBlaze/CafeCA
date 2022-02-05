using System;
using Pathfinding;

namespace Cafeo
{
    public class PlaceholderBrain : GenericBrain
    {
        public BattleVessel vessel;
        private AIPath _aiPath;

        public override BattleVessel Vessel
        {
            get => vessel;
            set => vessel = value;
        }

        public override void Start()
        {
            base.Start();
            _aiPath = GetComponent<AIPath>();
        }

        public override void DecideAction()
        {
            vessel.Move(_aiPath.steeringTarget - transform.position);
        }
    }
}