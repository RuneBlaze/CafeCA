using System;
using System.Collections.Generic;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo
{
    public class RogueManager : Singleton<RogueManager>
    {
        public List<BattleVessel> vessels = new();
        public BattleVessel player;
        private PlayerBrain _playerBrain;

        protected override void Setup()
        {
            base.Setup();
            Physics2D.simulationMode = SimulationMode2D.Script;
        }

        private void Start()
        {
            _playerBrain = player.GetComponent<PlayerBrain>();
        }

        public void RegisterVessel(BattleVessel vessel)
        {
            vessels.Add(vessel);
        }

        public void Update()
        {
            if (player.CanTakeAction)
            {
                if (!_playerBrain.PlayerDecideAction()) return;
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
    }
}