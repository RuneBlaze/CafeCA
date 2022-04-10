using UnityEngine;

namespace Cafeo.MapGen
{
    public class StartNode : MapNode
    {
        public StartNode(int id, Vector2Int position) : base(id, position)
        {
        }

        protected override void OnEnterState(State newState)
        {
            base.OnEnterState(newState);
            if (newState == State.Active)
            {
                ProgressState(); // start node is automatically cleared
            }
        }
    }
}