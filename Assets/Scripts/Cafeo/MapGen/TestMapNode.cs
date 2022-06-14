using UnityEngine;

namespace Cafeo.MapGen
{
    public class TestMapNode : MapNode
    {
        public TestMapNode(int id, Vector2Int position) : base(id, position)
        {
        }

        public override void AfterSpawned()
        {
            base.AfterSpawned();
            var dir = new[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            foreach (var d in dir) PlaceEnemySpawner("skeleton_archer", d * 3);
        }

        protected override void OnEnterState(State newState)
        {
            base.OnEnterState(newState);
            if (newState == State.Active) PlaceRoomClearer(Vector2.zero);
        }
    }
}