using System.Linq;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo.World
{
    public class TownPlaceholderBrain : TownBrain
    {
        public override void DecideAction()
        {
            base.DecideAction();
            // Debug.Log("deciding action");
            if (vessel.location is TownOuterNode exterior)
            {
                // Debug.Log("Random walking");
                var l = exterior.Neighbors.ToList();
                // Debug.Log($"{l.Count} neighbors");
                var n = l.RandomElement();
                vessel.action = new TownAction.Travel(vessel, 20, n);
            }
        }
    }
}