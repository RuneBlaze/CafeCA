using System;
using System.Collections.Generic;
using System.Globalization;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo.World
{
    public class TownPlayerBrain : TownBrain
    {
        private Queue<TownAction> actionQueue;

        private void Awake()
        {
            actionQueue = new Queue<TownAction>();
        }

        private void Update()
        {
            var a1 = Input.GetAxisRaw("Horizontal");
            var a2 = Input.GetAxisRaw("Vertical");
            if (a1 != 0 || a2 != 0)
            {
                var dir = new Vector2Int(VectorUtils.IntSgn(a1), VectorUtils.IntSgn(a2));
                var newLoc = Region.PlayerLoc + dir;
                TownOuterNode node;
                if (Region.IsValid(newLoc, out node))
                {
                    TryEnqueue(new TownAction.Travel(vessel, 2, node));
                }
            }
        }

        private void TryEnqueue(TownAction action)
        {
            while (actionQueue.Count >= 1)
            {
                actionQueue.Dequeue();
            }
            actionQueue.Enqueue(action);
        }

        public override void DecideAction()
        {
            base.DecideAction();
            if (actionQueue.Count > 0)
            {
                vessel.action = actionQueue.Dequeue();
            }
        }
    }
}