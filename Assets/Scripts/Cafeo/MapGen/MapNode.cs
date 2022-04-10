using System;
using UnityEngine;
using UnityEngine.Events;

namespace Cafeo.MapGen
{
    [Serializable]
    public class MapNode
    {
        public int id;
        public Vector2Int position;
        public State state;

        public RandomMapGenerator Map => RandomMapGenerator.Instance;
        
        public UnityEvent onStateChanged;
        public enum State
        {
            Unexplored,
            Active,
            Cleared,
        }
        public MapNode(int id, Vector2Int position)
        {
            this.id = id;
            this.position = position;
            this.state = State.Unexplored;
            this.onStateChanged = new UnityEvent();
        }

        protected virtual void OnExitState(State prevState)
        {
            
        }
        
        protected virtual void OnEnterState(State newState)
        {
            
        }

        public void ProgressState()
        {
            switch (state)
            {
                case State.Unexplored:
                    state = State.Active;
                    OnEnterState(State.Active);
                    break;
                case State.Active:
                    OnExitState(State.Active);
                    state = State.Cleared;
                    OnEnterState(State.Cleared);
                    break;
                case State.Cleared:
                    break;
            }
            onStateChanged.Invoke();
        }
    }
}