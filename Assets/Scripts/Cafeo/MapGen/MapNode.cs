using System;
using Cafeo.Data;
using Cafeo.Entities;
using Cafeo.Templates;
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
        public RogueManager Scene => RogueManager.Instance;
        
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
            counter = 0;
        }

        protected virtual void OnExitState(State prevState)
        {
            
        }
        
        protected virtual void OnEnterState(State newState)
        {
            if (newState == State.Cleared)
            {
                foreach (var vessel in root.GetComponentsInChildren<BattleVessel>())
                {
                    if (vessel.IsEnemy)
                    {
                        vessel.Kill();
                    }
                }
                AllyParty.Instance.AfterClear();
            }
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

        public void PlaceRoomClearer(Vector2 localPos)
        {
            var go = Map.SpawnRoomClearer((Vector2) root.transform.position + localPos);
            go.transform.SetParent(root);
        }

        public void PlaceChest(Vector2 localPos, DropInventory inventory)
        {
            var worldPos = (Vector2)root.transform.position + localPos;
            var go = Map.SpawnChest(worldPos, inventory);
            go.transform.SetParent(root);
        }

        public void PlaceEnemy(EnemyTemplate template, Vector2 localPos)
        {
            var go = Scene.SpawnBattleVessel(template, (Vector2) root.transform.position + localPos);
            go.transform.SetParent(root);
            counter++;
            // Debug.Log("On death hooked");
            go.onDeath.AddListener(() =>
            {
                counter--;
                Debug.Log(counter);
                if (counter <= 0)
                {
                    if (state == State.Active)
                    {
                        ProgressState();
                    }
                }
            });
        }

        public void PlaceEnemySpawner(string enemyId, Vector2 localPos)
        {
            var go = new GameObject("Spawner: " + enemyId);
            go.transform.SetParent(root);
            var enemySpawner = go.AddComponent<EnemySpawner>();
            enemySpawner.enemyId = enemyId;
            enemySpawner.LateInit(id);
            go.transform.localPosition = localPos;
        }
    }
}