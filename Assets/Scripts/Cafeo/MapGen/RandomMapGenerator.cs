using System.Linq;
using Cafeo.Data;
using Cafeo.Entities;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Cafeo.MapGen
{
    public class RandomMapGenerator : Singleton<RandomMapGenerator>
    {
        private const int blockSize = 17;
        public int mapSeed;
        public int mapWidth;
        public int mapHeight;
        [SerializeField] private TilemapPlacer tilemapPlacer;
        [SerializeField] private AstarPath aStarPath;
        [SerializeField] public Transform rogueDoorParent;

        [SerializeField] private GameObject roomClearerPrefab;
        [SerializeField] private GameObject chestPrefab;

        public int currentRoom = -1;

        public UnityEvent finishedSpawning;
        private RandomMap randomMap;

        private GameObject rogueDoorPrefab;


        private bool scanned;
        private float timer;

        public AllyParty Party => AllyParty.Instance;

        public RogueManager Scene => RogueManager.Instance;

        private void Start()
        {
            timer = 0.2f;
            randomMap = new RandomMap(mapWidth, mapHeight, mapSeed);
            Assert.IsNotNull(tilemapPlacer);
            rogueDoorPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Data/RoguePrefabs/RogueDoor.prefab")
                .WaitForCompletion();
            roomClearerPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Data/RoguePrefabs/RoomClearer.prefab")
                .WaitForCompletion();
            chestPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Data/RoguePrefabs/TreasureChest.prefab")
                .WaitForCompletion();
            randomMap.Generate();
            randomMap.WriteTextRepresentation();

            foreach (var mapNode in randomMap.Dfs())
            {
                // Debug.Log("Node: " + mapNode.id);
                var id = mapNode.id;
                var pos = mapNode.position;
                var roomPos = (pos - randomMap.startPoint) * blockSize;
                var roomRect = new RectInt(roomPos, new Vector2Int(blockSize, blockSize));
                tilemapPlacer.PlaceBox(roomRect);
                tilemapPlacer.EraseOpenings(roomRect, randomMap.edges[pos.x][pos.y]);
            }

            foreach (var (lhs, rhs) in randomMap.EachEdge())
            {
                // Debug.Log("Edge: " + randomMap.NumAlphaCode(lhs.id) + " " + randomMap.NumAlphaCode(rhs.id));
                var go = Instantiate(rogueDoorPrefab, rogueDoorParent);
                go.name = "Edge: " + randomMap.NumAlphaCode(lhs.id) + " " + randomMap.NumAlphaCode(rhs.id);
                var door = go.GetComponent<RoomDoor>();
                var horizontal = rhs.position.x == lhs.position.x;
                door.LateInit(lhs, rhs, horizontal);
                var posX = (lhs.position.x + rhs.position.x) / 2f;
                var posY = (lhs.position.y + rhs.position.y) / 2f;
                go.transform.position =
                    (new Vector3(posX, posY, 0) - new Vector3(randomMap.startPoint.x, randomMap.startPoint.y)) *
                    blockSize + new Vector3(blockSize, blockSize) / 2f;
                if (horizontal)
                    go.transform.position += Vector3.up * 0.5f;
                else
                    go.transform.position += Vector3.right * 0.5f;
            }

            Scene.InitializeBattleParty();

            foreach (var ally in Scene.Allies())
                if (ally.IsLeaderAlly)
                    ally.transform.position = MapCoord2WorldCoord(randomMap.startPoint);
                else
                    ally.transform.position = MapCoord2WorldCoord(randomMap.startPoint) + Random.insideUnitCircle * 2f;

            for (var i = 1; i < randomMap.nodes.Count; i++) randomMap.nodes[i].AfterSpawned();

            Scene.rogueUpdateEvent.AddListener(RogueUpdate);

            // aStarPath.UpdateGraphs(new Bounds(Vector3.zero, new Vector3(mapWidth, mapHeight, 0)));
        }

        private void Update()
        {
        }

        private void LateUpdate()
        {
            if (!scanned)
            {
                scanned = true;
                Rescan();
            }
            // Debug.Log(World2Room(Scene.leaderAlly.transform.position));
        }

        protected override void Setup()
        {
            base.Setup();
            finishedSpawning = new UnityEvent();
        }

        public Vector2 MapCoord2WorldCoord(Vector2Int mapCoord)
        {
            return new Vector2(mapCoord.x * blockSize, mapCoord.y * blockSize) -
                   new Vector2(randomMap.startPoint.x, randomMap.startPoint.y) * blockSize +
                   new Vector2(blockSize, blockSize) / 2f;
        }

        public MapNode NodeById(int i)
        {
            return randomMap.nodes[i];
        }

        private void Rescan()
        {
            var gridGraph = AstarPath.active.data.gridGraph;
            // gridGraph.width = 150;
            // gridGraph.depth = 150;
            gridGraph.SetDimensions(150, 150, 1);
            AstarPath.active.Scan(gridGraph);
        }

        public int World2Room(Vector2 pos)
        {
            // return new Vector2(mapCoord.x * blockSize, mapCoord.y * blockSize) - 
            //     (new Vector2(randomMap.startPoint.x, randomMap.startPoint.y)) * blockSize + new Vector2(blockSize, blockSize) / 2f;
            var normalized = pos / blockSize + new Vector2(randomMap.startPoint.x, randomMap.startPoint.y);
            var nx = normalized.x;
            var ny = normalized.y;
            // const float threshold = 0.2f;
            var f = 0.12f;
            if ((int)(nx - f) != (int)nx || (int)(ny - f) != (int)ny) return -1;

            if ((int)(nx + f) != (int)nx || (int)(ny + f) != (int)ny) return -1;


            var ix = (int)nx;
            var iy = (int)ny;
            // if (!randomMap.IsPointValid(new Vector2Int(ix, iy)))
            // {
            //     return -1;
            // }

            if (randomMap.IsOccupied(new Vector2Int(ix, iy))) return randomMap.geometry[ix][iy];

            return -1;
        }

        public int AllyCurrentRoom()
        {
            var positions = Scene.Allies().Select(it => World2Room(it.transform.position)).ToList();
            // return the consensus of the positions or -1 if there is no consensus
            var first = positions.First();
            for (var i = 0; i < positions.Count; i++)
            {
                if (positions[i] != first) return -1;

                if (positions[i] == -1) return -1;
            }

            return first;
        }

        private void RogueUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= 0.2f)
            {
                timer = 0;
                var room = AllyCurrentRoom();
                if (room > 0)
                {
                    currentRoom = room;
                    var node = randomMap.nodes[room];
                    if (node.state == MapNode.State.Unexplored)
                    {
                        node.ProgressState();
                        Party.BeforeRoom();
                    }
                }
            }

            if (currentRoom != -1)
            {
                var node = randomMap.nodes[currentRoom];
                if (node.state == MapNode.State.Active && node.counter <= 0)
                {
                    node.ProgressState();
                    Party.BeforeRoom();
                }
            }
        }

        public GameObject SpawnRoomClearer(Vector2 pos)
        {
            var go = Instantiate(roomClearerPrefab, rogueDoorParent);
            go.transform.position = pos;
            return go;
        }

        public GameObject SpawnChest(Vector2 pos, DropInventory inventory)
        {
            var go = Instantiate(chestPrefab, rogueDoorParent);
            go.transform.position = pos;
            var chest = go.GetComponent<TreasureChest>();
            chest.inventory = inventory;
            return go;
        }
    }
}