using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using Cafeo.Castable;
using Cafeo.Data;
using Cafeo.Entities;
using Cafeo.Gadgets;
using Cafeo.Templates;
using Cafeo.Utils;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Cafeo
{
    public class RogueManager : Singleton<RogueManager>
    {
        public List<BattleVessel> vessels = new();
        public List<BattleVessel> otherAllyVessels = new();
        public BattleVessel player;
        private PlayerBrain playerBrain;
        private GameObject popupPrefab;
        private GameObject collectablePrefab;
        [HideInInspector] public GameObject leaderAlly;
        public UnityEvent rogueUpdateEvent = new();
        [SerializeField] private Transform popupParent;
        [SerializeField] private  Transform collectableParent;

        public bool inputLocked;

        public List<BehaviorTree> allyBehaviorTrees = new();
        public List<BehaviorTree> enemyBehaviorTrees = new();

        public Dictionary<int, BattleVessel> goIdToVessel = new();

        protected override void Setup()
        {
            base.Setup();
            inputLocked = false;
            Physics2D.simulationMode = SimulationMode2D.Script;
            popupPrefab = Addressables
                .LoadAssetAsync<GameObject>("Assets/Data/RoguePrefabs/Popup.prefab")
                .WaitForCompletion();
            collectablePrefab = Addressables
                .LoadAssetAsync<GameObject>("Assets/Data/RoguePrefabs/Collectable.prefab")
                .WaitForCompletion();
        }

        public IEnumerable<BattleVessel> Allies()
        {
            yield return player;
            foreach (var vessel in otherAllyVessels)
            {
                yield return vessel;
            }
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

            int dead = 0;
            foreach (var vessel in vessels)
            {
                if (vessel != null)
                {
                    vessel.RogueUpdate();
                }
                else
                {
                    dead++;
                }
            }
            
            if (dead > 0)
            {
                vessels.RemoveAll(x => x == null);
            }
            
            rogueUpdateEvent.Invoke();
            BehaviorManager.instance.Tick();
            Physics2D.Simulate(Time.deltaTime);
        }

        public Projectile CreateProjectile(
            ProjectileType type, BattleVessel owner, Vector2 position, Vector2 direction)
        {
            var go = new GameObject("projectile");
            type.shape.CreateCollider(go);
            go.AddComponent<Rigidbody2D>();
            var proj = go.AddComponent<Projectile>();
            proj.type = type;
            proj.owner = owner;
            proj.deltaSize = type.deltaSize;
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
                var r = CreateProjectile(type, owner, position, dir);
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

        public void SpawnDroppable(Vector2 position, IDroppable droppable, Vector2 initialForce)
        {
            var go = Instantiate(collectablePrefab, collectableParent);
            go.transform.position = position;
            var collectable = go.GetComponent<Collectable>();
            collectable.LateInit(droppable);
            collectable.BeThrown(initialForce);
        }

        public void SpawnDroppable(Vector2 position, IDroppable droppable)
        {
            SpawnDroppable(position, droppable, Vector2.zero);
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

        public float CalculateBaseDamageMelee(BattleVessel attacker, BattleVessel defender, UsableItem skill, bool arts = false)
        {
            var lhs = attacker;
            var rhs = defender;
            var x = skill.power / 100f;
            if (arts)
            {
                return x * (lhs.Mat * 1.8f - rhs.Mdf);
            }

            return x * (lhs.Atk * 1.8f - rhs.Def);
        }
        
        public float CalculateDamageMelee(BattleVessel attacker, BattleVessel defender, UsableItem skill, bool arts = false)
        {
            var baseDamage = CalculateBaseDamageMelee(attacker, defender, skill, arts);
            return baseDamage * CalcVariance(attacker, defender, skill);
        }
        
        public float CalculateBaseDamageRanged(BattleVessel attacker, BattleVessel defender, UsableItem skill, bool arts = false)
        {
            var lhs = attacker;
            var rhs = defender;
            var x = skill.power / 100f;
            if (arts)
            {
                return x * (lhs.Mat * 1.4f + lhs.Dex * 1.8f - rhs.Mdf);
            }

            return x * (lhs.Atk * 1.4f + lhs.Dex * 1.8f - rhs.Def);
        }
        
        public float CalculateDamageRanged(BattleVessel attacker, BattleVessel defender, UsableItem skill, bool arts = false)
        {
            var baseDamage = CalculateBaseDamageRanged(attacker, defender, skill, arts);
            return baseDamage * CalcVariance(attacker, defender, skill);
        }
        
        public float CalculateBaseHeal(BattleVessel attacker, BattleVessel defender, UsableItem skill)
        {
            var lhs = attacker;
            // var rhs = defender.soul;
            var x = skill.power / 100f;
            return x * (lhs.Mat * 1.2f);
        }
        
        public float CalculateHeal(BattleVessel attacker, BattleVessel defender, UsableItem skill)
        {
            var baseHeal = CalculateBaseHeal(attacker, defender, skill);
            return baseHeal * CalcVariance(attacker, defender, skill);
        }
        
        public float CalcVariance(BattleVessel attacker, BattleVessel defender, UsableItem skill)
        {
            return Random.Range(0.8f, 1.2f);
        }

        public void OnDebugEnabled()
        {
            inputLocked = true;
        }
        
        public void OnDebugDisabled()
        {
            inputLocked = false;
        }
        
        public BattleVessel SpawnBattleVessel(EnemyTemplate template, Vector2 pos)
        {
            throw new System.NotImplementedException();
        }
    }
}