using System;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;

namespace Cafeo
{
    public class PlaceholderBrain : GenericBrain
    {
        public BattleVessel vessel;
        private AIPath _aiPath;
        private RVOController _rvoController;

        public override BattleVessel Vessel
        {
            get => vessel;
            set => vessel = value;
        }

        public override void Start()
        {
            base.Start();
            _aiPath = GetComponent<AIPath>();
            _rvoController = GetComponent<RVOController>();

            
        }

        public override void DecideAction()
        {
            if (_aiPath.hasPath)
            {
                var dir = _aiPath.steeringTarget - transform.position;
                _rvoController.SetTarget(transform.position + dir * 3, 2, 2);
                var delta = _rvoController.CalculateMovementDelta(transform.position, Time.deltaTime);
                vessel.Move(delta);
            }
        }
    }
}