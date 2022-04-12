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
        public Transform root;

        public int counter; // upon counter reaching zero, this room is completed

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

        public virtual void AfterSpawned()
        {
            var go = new GameObject();
            go.name = $"Room {id}";
            go.transform.position = WorldPos;
            go.transform.SetParent(Map.rogueDoorParent);
            root = go.transform;
        }

        public Vector2 WorldPos => Map.MapCoord2WorldCoord(position);

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

        public void PlaceRoomClearer(Vector2 pos)
        {
            var go = Map.SpawnRoomClearer((Vector2) root.transform.position + pos);
            go.transform.SetParent(root);
        }
    }
}