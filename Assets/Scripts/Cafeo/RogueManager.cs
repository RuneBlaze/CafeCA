﻿using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using Cafeo.Gadgets;
using Cafeo.Utils;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Cafeo
{
    public class RogueManager : Singleton<RogueManager>
    {
        public List<BattleVessel> vessels = new();
        public List<BattleVessel> otherAllyVessels = new();
        public BattleVessel player;
        private PlayerBrain playerBrain;
        private GameObject popupPrefab;
        [HideInInspector] public GameObject leaderAlly;
        public UnityEvent rogueUpdateEvent = new();
        [SerializeField] private Transform popupParent;

        public List<BehaviorTree> allyBehaviorTrees = new();
        public List<BehaviorTree> enemyBehaviorTrees = new();
        
        public Dictionary<int, BattleVessel> goIdToVessel = new();

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
            Assert.IsNotNull(player);
            playerBrain = player.GetComponent<PlayerBrain>();
            leaderAlly = CalcLeaderAlly().gameObject;
            foreach (var battleVessel in vessels)
            {
                if (battleVessel != player && battleVessel.IsAlly)
                {
                    otherAllyVessels.Add(battleVessel);
                }

                // if (battleVessel.IsAlly)
                // {
                //     var tree = battleVessel.GetComponent<BehaviorTree>();
                //     if (tree != null) allyBehaviorTrees.Add(tree);
                // }
                // else
                // {
                //     var tree = battleVessel.GetComponent<BehaviorTree>();
                //     if (tree != null) enemyBehaviorTrees.Add(tree);
                // }
            }
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
            // then we tick everyone else
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
            rogueUpdateEvent.Invoke();
            BehaviorManager.instance.Tick();
            Physics2D.Simulate(Time.deltaTime);
        }

        public Projectile CreateProjectiles(
            ProjectileType type, BattleVessel owner, Vector2 position, Vector2 direction)
        {
            var go = new GameObject("projectile");
            type.shape.CreateCollider(go);
            go.AddComponent<Rigidbody2D>();
            var proj = go.AddComponent<Projectile>();
            proj.type = type;
            proj.owner = owner;
            proj.sizeDelta = type.deltaSize;
            proj.pierce = type.pierce;
            proj.bounce = type.bounce;
            proj.initialDirection = direction;
            go.transform.position = position;
            proj.Setup();
            return proj;
        }

        public List<Projectile> CreateFanProjectiles(
            ProjectileType type, int k, int angle, BattleVessel owner, Vector2 position, Vector2 direction)
        {
            float inc = (float) angle / (k - 1);
            var projs = new List<Projectile>();
            for (int i = 0; i < k; i++)
            {
                var dir = 
                    Quaternion.AngleAxis(-angle / 2f + inc * i, Vector3.forward) * direction.normalized;
                var r = CreateProjectiles(type, owner, position, dir);
                projs.Add(r);
            }
            return projs;
        }

        public void CreatePopup(Vector2 position, string text, Color color)
        {
            var go = Instantiate(popupPrefab, popupParent);
            go.transform.position = position;
            var popup = go.GetComponent<Popup>();
            popup.textColor = color;
            popup.SetText(text);
        }

        public BattleVessel CalcLeaderAlly()
        {
            return player;
        }

        public void CreatePopup(Vector2 position, string text)
        {
            CreatePopup(position, text, Color.red);
        }

        public BattleVessel GetVesselFromGameObject(GameObject go)
        {
            var id = go.GetInstanceID();
            if (goIdToVessel.ContainsKey(id))
            {
                return goIdToVessel[id];
            }
            else
            {
                goIdToVessel[id] = go.GetComponent<BattleVessel>();
                return goIdToVessel[id];
            }
        }
    }
}