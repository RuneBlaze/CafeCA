using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using Cafeo.Castable;
using Cafeo.Configs;
using Cafeo.Data;
using Cafeo.Entities;
using Cafeo.Gadgets;
using Cafeo.Templates;
using Cafeo.Utils;
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
        [HideInInspector] public GameObject leaderAlly;
        public UnityEvent rogueUpdateEvent = new();
        [SerializeField] private Transform popupParent;
        [SerializeField] private Transform collectableParent;
        [SerializeField] private Transform enemiesSoulParent;
        [SerializeField] private Transform agentParent;

        public bool inputLocked;

        public List<BehaviorTree> allyBehaviorTrees = new();
        public List<BehaviorTree> enemyBehaviorTrees = new();

        public Sprite coinSprite;
        public Sprite keySprite;
        private GameObject agentPrefab;
        private GameObject collectablePrefab;
        private GameObject entityLabelPrefab;

        public Dictionary<int, BattleVessel> goIdToVessel = new();
        private PlayerBrain playerBrain;
        private GameObject popupPrefab;

        private void Start()
        {
            Assert.IsNotNull(player);
            playerBrain = player.GetComponent<PlayerBrain>();
            leaderAlly = CalcLeaderAlly().gameObject;
            foreach (var battleVessel in vessels)
                if (battleVessel != player && battleVessel.IsAlly)
                    otherAllyVessels.Add(battleVessel);

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

        public void Update()
        {
            if (player.CanTakeAction)
                if (!playerBrain.PlayerDecideAction())
                    return;
            // then we tick everyone else
            foreach (var vessel in vessels)
                if (vessel != player && vessel.CanTakeAction)
                    vessel.Brain.DecideAction();

            var dead = 0;
            foreach (var vessel in vessels)
                if (vessel != null)
                    vessel.RogueUpdate();
                else
                    dead++;

            if (dead > 0) vessels.RemoveAll(x => x == null);

            rogueUpdateEvent.Invoke();
            BehaviorManager.instance.Tick();
            Physics2D.Simulate(Time.deltaTime);
        }

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
            agentPrefab = Addressables
                .LoadAssetAsync<GameObject>("Assets/Data/RoguePrefabs/GenericAgent.prefab")
                .WaitForCompletion();
            coinSprite = Addressables
                .LoadAssetAsync<Sprite>("Assets/Graphics/Icons/Debug/Icons_12.png")
                .WaitForCompletion();
            keySprite = Addressables
                .LoadAssetAsync<Sprite>("Assets/Graphics/Icons/Debug/Icons_09.png")
                .WaitForCompletion();
            entityLabelPrefab = Addressables
                .LoadAssetAsync<GameObject>("Assets/Data/RoguePrefabs/EntityLabel.prefab")
                .WaitForCompletion();
        }

        public IEnumerable<BattleVessel> Allies()
        {
            yield return player;
            foreach (var vessel in otherAllyVessels) yield return vessel;
        }

        public void InitializeBattleParty()
        {
            Debug.Log("Initializing battle party");
            var initialParty = InitialParty.Instance;
            var inventories = initialParty.battleInventories;
            var allies = Allies().ToList();
            Assert.IsTrue(allies.Count == inventories.Count);
            for (var i = 0; i < inventories.Count; i++)
            {
                allies[i].ClearHotbar();
                var config = inventories[i];
                var j = 0;
                foreach (var skill in config.restSkills)
                {
                    var item = skill.Generate();
                    allies[i].hotbar[j] = item;
                    j++;
                }

                allies[i].SetHotboxPointer(0);
            }
        }

        public void RegisterVessel(BattleVessel vessel)
        {
            vessels.Add(vessel);
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
            var inc = (float)angle / (k - 1);
            var projs = new List<Projectile>();
            for (var i = 0; i < k; i++)
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

        public Collectable SpawnDroppable(Vector2 position, IDroppable droppable, Vector2 initialForce)
        {
            var go = Instantiate(collectablePrefab, collectableParent);
            go.transform.position = position;
            var collectable = go.GetComponent<Collectable>();
            collectable.LateInit(droppable);
            collectable.BeThrown(initialForce);
            return collectable;
        }

        public void SpawnCoin(Vector2 position, Vector2 initialForce)
        {
            var go = Instantiate(collectablePrefab, collectableParent);
            go.transform.position = position;
            var collectable = go.GetComponent<Collectable>();
            collectable.LateInit(new SmallCoin());
            collectable.BeThrown(initialForce);
        }

        public void SpawnKey(Vector2 position, Vector2 initialForce)
        {
            var go = Instantiate(collectablePrefab, collectableParent);
            go.transform.position = position;
            var collectable = go.GetComponent<Collectable>();
            collectable.LateInit(new DroppedKey());
            collectable.BeThrown(initialForce);
        }

        public void PopDropInventory(Vector2 position, DropInventory inventory, Vector2 direction)
        {
            foreach (var charm in inventory.charms) SpawnDroppable(position, charm, direction);

            foreach (var treasure in inventory.treasures) SpawnDroppable(position, treasure, direction);

            foreach (var oneTimeUseItem in inventory.oneTimeUseItems)
                SpawnDroppable(position, oneTimeUseItem, direction);

            for (var i = 0; i < inventory.coins; i++) SpawnCoin(position, direction);

            for (var i = 0; i < inventory.keys; i++) SpawnKey(position, direction);
        }

        public Collectable SpawnDroppable(Vector2 position, IDroppable droppable)
        {
            return SpawnDroppable(position, droppable, Vector2.zero);
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

            goIdToVessel[id] = go.GetComponent<BattleVessel>();
            return goIdToVessel[id];
        }

        public float CalculateBaseDamageMelee(BattleVessel attacker, BattleVessel defender, UsableItem skill,
            bool arts = false)
        {
            var lhs = attacker;
            var rhs = defender;
            var x = skill.power / 100f;
            if (arts) return x * (lhs.Mat * 1.8f - rhs.Mdf);

            return x * (lhs.Atk * 1.8f - rhs.Def);
        }

        public float CalculateDamageMelee(BattleVessel attacker, BattleVessel defender, UsableItem skill,
            bool arts = false)
        {
            var baseDamage = CalculateBaseDamageMelee(attacker, defender, skill, arts);
            return baseDamage * CalcVariance(attacker, defender, skill);
        }

        public float CalculateBaseDamageRanged(BattleVessel attacker, BattleVessel defender, UsableItem skill,
            bool arts = false)
        {
            var lhs = attacker;
            var rhs = defender;
            var x = skill.power / 100f;
            if (arts) return x * (lhs.Mat * 1.4f + lhs.Dex * 1.8f - rhs.Mdf);

            return x * (lhs.Atk * 1.4f + lhs.Dex * 1.8f - rhs.Def);
        }

        public float CalculateDamageRanged(BattleVessel attacker, BattleVessel defender, UsableItem skill,
            bool arts = false)
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
            // first create the soul
            var soulGo = new GameObject(template.displayName);
            soulGo.transform.SetParent(enemiesSoulParent);
            var soul = template.AddToGameObjet(soulGo);

            // then create the vessel
            var agentGo = Instantiate(agentPrefab, agentParent);
            var agent = agentGo.GetComponent<BattleVessel>();
            agent.soul = soul;
            agent.aiType = template.aiType;
            agent.transform.position = pos;
            template.ModifyVessel(agent);
            return agent;
        }

        public TextMesh AttachLabel(Transform parent)
        {
            var go = Instantiate(entityLabelPrefab, parent);
            var label = go.GetComponent<TextMesh>();
            go.transform.localPosition = Vector3.zero + Vector3.down * 0.5f;
            return label;
        }
    }
}