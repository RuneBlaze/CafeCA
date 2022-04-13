using System;
using Cafeo.MapGen;
using UnityEngine;

namespace Cafeo.Entities
{
    public class BaseSpawner : MonoBehaviour
    {
        public int roomId;
        
        public RandomMapGenerator Map => RandomMapGenerator.Instance;
        public virtual MapNode.State ListenToState { get; set; }

        public Vector2 RelPos => (Vector2) transform.position -  MyNode.WorldPos;

        public void LateInit(int roomId)
        {
            this.roomId = roomId;
            ListenToState = MapNode.State.Active;
        }
        
        public void Start()
        {
            OnFinishedSpawning();
        }

        public MapNode MyNode => Map.NodeById(roomId);
        
        public void OnFinishedSpawning()
        {
            MyNode.onStateChanged.AddListener(() =>
            {
                OnNodeStateChanged(MyNode.state);
            });
            MyNode.counter++;
        }

        private void OnNodeStateChanged(MapNode.State myNodeState)
        {
            if (myNodeState == ListenToState)
            {
                Activate();
                AfterActivation();
            }
        }

        protected virtual void AfterActivation()
        {
            Destroy(gameObject);
        }

        protected virtual void Activate()
        {
            MyNode.counter--;
        }
    }
}