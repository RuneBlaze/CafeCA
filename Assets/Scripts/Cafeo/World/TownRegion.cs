using System;
using System.Collections.Generic;
using Cafeo.Templates;
using Cafeo.Utils;
using Drawing;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Cafeo.World
{
    /// <summary>
    /// God object of the WorldScene
    /// </summary>
    public class TownRegion : Singleton<TownRegion>
    {
        private const int SquareSize = 16;
        public Transform worldRoot;
        public Transform soulRoot;
        public int width;
        public int height;
        public TownVessel player;
        public List<TownVessel> vessels;
        public List<AgentSoul> souls;
        private Camera cam;
        public List<(Rect, Color)> miniMap;
        public TownOuterNode[,] outerNodes;
        public float tickRate = 0.5f;
        private float timer = 0f;
        public bool idleMode;
        public UnityEvent<TownNode> onPlayerMove;
        public UnityEvent initFinished;
        public Vector2Int PlayerLoc => new Vector2Int(player.outerNode.x, player.outerNode.y);

        public void LateUpdate()
        {
            var draw = Draw.ingame;
            using (draw.InScreenSpace(cam))
            {
                var i = 0;
                for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    var (rect, color) = miniMap[i];
                    var playerLoc = PlayerLoc;
                    if (x == playerLoc.x && y == playerLoc.y)
                    {
                        draw.SolidRectangle(rect, Color.black);
                    }
                    else
                    {
                        draw.SolidRectangle(rect, color);
                    }
                    i++;
                }
                // foreach (var valueTuple in miniMap)
                // {
                //     var (rect, color) = valueTuple;
                //     draw.SolidRectangle(rect, color);
                // }
            }
        }

        public void ToggleIdle()
        {
            idleMode = !idleMode;
        }

        private void Update()
        {
            TickIfNecessary();
        }

        private void TickIfNecessary()
        {
            timer += Time.deltaTime;
            if (timer >= tickRate)
            {
                timer -= tickRate;
                if (player.Idle &&! idleMode)
                {
                    
                }
                else
                {
                    // Debug.Log("Ticking!");
                    EveryoneAct();
                    Clock.ElapseTurn();
                }
            }
        }

        private void EveryoneAct()
        {
            foreach (var vessel in vessels)
            {
                if (vessel == player) continue;
                vessel.OnTurn();
            }
        }

        public GameClock Clock => GameClock.Instance;

        protected override void Setup()
        {
            base.Setup();
            width = 8;
            height = 8;
            vessels = new List<TownVessel>();
            souls = new List<AgentSoul>();
            miniMap = new List<(Rect, Color)>();
            onPlayerMove = new UnityEvent<TownNode>();
            initFinished = new UnityEvent();
        }
        private void Start()
        {
            cam = Camera.main;
            Generate();
            PopulateSouls();
            PlaceSouls();
            SetupAI();
            Refresh();
            initFinished.Invoke();
        }

        private void SetupAI()
        {
            foreach (var townVessel in vessels)
            {
                if (townVessel != player)
                {
                    townVessel.gameObject.AddComponent<TownPlaceholderBrain>();
                }
                else
                {
                    townVessel.gameObject.AddComponent<TownPlayerBrain>();
                }
            }
        }

        public void PopulateSouls()
        {
            var finder = TemplateFinder.Instance;
            for (int i = 0; i < 100; i++)
            {
                var tmpl = finder.RetrieveTemplate<SoulTemplate>("soul:default");
                var go = new GameObject($"Soul {i}");
                go.transform.parent = soulRoot;
                var v = tmpl.AddToGameObjet(go);
                v.firstName = $"Jon {Random.Range(0, 50)}";
                v.lastName = "Doe";
                souls.Add(v);
            }
        }

        private void PlaceSouls()
        {
            foreach (var agentSoul in souls)
            {
                // let's just randomly place them
                var node = GetRandomOuterNode();
                var go = new GameObject();
                go.transform.parent = node.transform;
                go.tag = "TownAgent";
                go.name = $"Vessel [{agentSoul.firstName} {agentSoul.lastName}]";
                var vessel = go.AddComponent<TownVessel>();
                vessel.Subscribe();
                vessels.Add(vessel);
                vessel.soul = agentSoul;
            }
            player = vessels[0];
            player.name = "@Player";
        }
        
        public (int, int) GetRandomPosition()
        {
            var x = Random.Range(0, width);
            var y = Random.Range(0, height);
            return (x, y);
        }
        
        public TownOuterNode GetRandomOuterNode()
        {
            var (x, y) = GetRandomPosition();
            return outerNodes[x, y];
        }

        public TownOuterNode CreateOuterNode(int x, int y)
        {
            var go = new GameObject
            {
                transform =
                {
                    parent = worldRoot
                },
                name = $"[{x}_{y}]"
            };
            go.tag = "TownExterior";
            var outerNode = go.AddComponent<TownOuterNode>();
            outerNode.region = this;
            outerNode.x = x;
            outerNode.y = y;
            return outerNode;
        }

        protected RealEstateDev ChooseDeveloper(int x, int y)
        {
            var i = Random.Range(0, 6);
            return i switch
            {
                0 => new RealEstateDev.Mall(),
                1 => new RealEstateDev.Apartments(),
                2 => new RealEstateDev.Road(),
                3 => new RealEstateDev.School(),
                4 => new RealEstateDev.HighSchool(),
                5 => new RealEstateDev.SuburbanHouses(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Generate()
        {
            Assert.IsNotNull(worldRoot);
            outerNodes = new TownOuterNode[width, height];
            // let's just do shuffle generation for now
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                outerNodes[x, y] = CreateOuterNode(x, y);
                var dev = ChooseDeveloper(x, y);
                dev.Furnish(outerNodes[x, y]);
            }

            CreateMiniMap();
        }

        private void CreateMiniMap()
        {
            int baseX = 50;
            int baseY = 500;
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var n = outerNodes[x, y];
                miniMap.Add(
                    (new Rect(x * SquareSize + baseX, y * SquareSize + baseY, SquareSize, SquareSize), 
                    n.color));
            }
        }

        public bool IsValid(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public bool IsValid(Vector2Int loc)
        {
            return IsValid(loc.x, loc.y);
        }

        public bool IsValid(Vector2Int loc, out TownOuterNode node)
        {
            if (IsValid(loc))
            {
                node = outerNodes[loc.x, loc.y];
                return true;
            }
            node = null;
            return false;
        }
        
        public static readonly (int, int)[] cardinalDir = {
            (0, -1),
            (0, 1),
            (-1, 0),
            (1, 0)
        };

        public IEnumerable<TownOuterNode> OuterNeighbors(TownOuterNode node)
        {
            foreach (var (l, x, r) in OuterNeighborsWithRelCoord(node))
            {
                yield return l;
            }
        }
        
        public IEnumerable<(TownOuterNode, int, int)> OuterNeighborsWithRelCoord(TownOuterNode node)
        {
            foreach (var (x, y) in cardinalDir)
            {
                if (IsValid(node.x + x, node.y + y))
                {
                    yield return (outerNodes[node.x + x, node.y + y], x, y);
                }
            }
        }

        public void Refresh()
        {
            foreach (var townOuterNode in outerNodes)
            {
                townOuterNode.Refresh();
            }
        }
    }
}