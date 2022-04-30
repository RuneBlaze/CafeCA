using Cafeo.Data;
using UnityEngine;

namespace Cafeo.MapGen
{
    public class StartNode : MapNode
    {
        public StartNode(int id, Vector2Int position) : base(id, position)
        {
            counter = 0;
        }

        protected override void OnEnterState(State newState)
        {
            base.OnEnterState(newState);
            if (newState == State.Active)
            {
                PlaceChest(Vector2.up * 3, DropInventory.CoinsAndKeys(3,3)); // DEBUG
                ProgressState(); // start node is automatically cleared
            }
        }
    }
}