using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using Cafeo.Gadgets;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        public GameObject leaderAlly;
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
            Physics2D.Simulate(Time.deltaTime);
        }

        public Projectile CreateProjectiles(ProjectileType type, BattleVessel owner, Vector2 position, Vector2 direction)
        {
            var go = new GameObject("projectile");
            type.Shape.CreateCollider(go);
            go.AddComponent<Rigidbody2D>();
            var proj = go.AddComponent<Projectile>();
            proj.type = type;
            proj.owner = owner;
            proj.sizeDelta = type.deltaSize;
            proj.pierce = type.pierce;
            proj.initialDirection = direction;
            proj.Setup();
            go.transform.position = position;
            return proj;
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