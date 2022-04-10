using UnityEngine;

namespace Cafeo.MapGen
{
    public class TestMapNode : MapNode
    {
        public TestMapNode(int id, Vector2Int position) : base(id, position)
        {
        }

        protected override void OnEnterState(State newState)
        {
            base.OnEnterState(newState);
            if (newState == State.Active)
            {
                Map.SpawnRoomClearer(Map.MapCoord2WorldCoord(position));
            }
        }
    }
}