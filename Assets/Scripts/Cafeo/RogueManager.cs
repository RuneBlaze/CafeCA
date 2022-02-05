using System;
using System.Collections.Generic;
using Cafeo.Gadgets;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Cafeo
{
    public class RogueManager : Singleton<RogueManager>
    {
        public List<BattleVessel> vessels = new();
        public BattleVessel player;
        private PlayerBrain playerBrain;

        private GameObject popupPrefab;
        [SerializeField] private Transform popupParent;

        protected override void Setup()
        {
            base.Setup();
            Physics2D.simulationMode = SimulationMode2D.Script;
            popupPrefab = Addressables
                .LoadAssetAsync<GameObject>("Assets/Data/RoguePrefabs/Popup.prefab")
                .WaitForCompletion();
        }

        private void Start()
        {
            playerBrain = player.GetComponent<PlayerBrain>();
        }

        public void RegisterVessel(BattleVessel vessel)
        {
            vessels.Add(vessel);
        }

        public void Update()
        {
            if (player.CanTakeAction)
            {
                if (!playerBrain.PlayerDecideAction()) return;
            }
            foreach (var vessel in vessels)
            {
                if (vessel != player && vessel.CanTakeAction)
                {
                    vessel.Brain.DecideAction();
                }
            }
            foreach (var vessel in vessels)
            {
                vessel.RogueUpdate();
            }
            Physics2D.Simulate(Time.deltaTime);
        }

        public void CreateProjectiles(ProjectileType type, BattleVessel owner, Vector2 position, Vector2 direction)
        {
            var go = new GameObject("projectile");
            type.Shape.CreateCollider(go);
            go.AddComponent<Rigidbody2D>();
            var proj = go.AddComponent<Projectile>();
            proj.type = type;
            proj.owner = owner;
            proj.initialDirection = direction;
            proj.Setup();
            go.transform.position = position;
        }

        public void CreatePopup(Vector2 position, string text)
        {
            var go = Instantiate(popupPrefab, popupParent);
            go.transform.position = position;
            var popup = go.GetComponent<Popup>();
            popup.textColor = Color.red;
            popup.SetText(text);
        }
    }
}