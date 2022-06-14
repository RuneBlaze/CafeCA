using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cafeo.MapGen
{
    public class RandomMap
    {
        [Flags]
        public enum Edges
        {
            None = 0,
            Top = 1 << 1,
            Bottom = 1 << 2,
            Left = 1 << 3,
            Right = 1 << 4
        }

        public Edges[][] edges; // mapping from coordinate to a graph node

        // FIXME: forgot to use actual 2d arrays
        public int[][] geometry; // mapping from coordinate to a graph node
        public int H;
        private readonly MarkovDigger markovDigger;

        public List<MapNode> nodes;
        // public int seed;

        public Vector2Int startPoint;
        public int W;

        public RandomMap(int w, int h, int seed = 0)
        {
            W = w;
            H = h;
            // this.seed = seed;
            Random.InitState(seed);
            geometry = new int[W][];
            edges = new Edges[W][];
            nodes = new List<MapNode> { null }; // the first node is reserved
            for (var i = 0; i < W; i++)
            {
                geometry[i] = new int[H];
                edges[i] = new Edges[H];
            }

            markovDigger = new MarkovDigger();
        }

        public Vector2Int RotateLeft(Vector2Int dir)
        {
            // rotate a direction to the left by 90 degrees
            return new Vector2Int(dir.y, -dir.x);
        }

        public Vector2Int RotateRight(Vector2Int dir)
        {
            // rotate a direction to the right by 90 degrees
            return new Vector2Int(-dir.y, dir.x);
        }

        public Edges FromDir(Vector2Int dir)
        {
            return dir.x switch
            {
                1 => Edges.Right,
                -1 => Edges.Left,
                _ => dir.y switch
                {
                    1 => Edges.Top,
                    -1 => Edges.Bottom,
                    _ => Edges.None
                }
            };
        }

        public Edges Opposite(Edges fromEdges)
        {
            return fromEdges switch
            {
                Edges.Top => Edges.Bottom,
                Edges.Bottom => Edges.Top,
                Edges.Left => Edges.Right,
                Edges.Right => Edges.Left,
                _ => Edges.None
            };
        }

        public bool IsPointValid(Vector2Int point)
        {
            return point.x >= 0 && point.x < W && point.y >= 0 && point.y < H;
        }

        public bool IsPassable(Vector2Int point)
        {
            return IsPointValid(point) && geometry[point.x][point.y] == 0;
        }

        public bool IsOccupied(Vector2Int point)
        {
            return IsPointValid(point) && !IsPassable(point);
        }

        // if the point is surrounded by not passable points
        private bool IsStuck(Vector2Int point)
        {
            return !IsPassable(point + new Vector2Int(1, 0)) &&
                   !IsPassable(point + new Vector2Int(-1, 0)) &&
                   !IsPassable(point + new Vector2Int(0, 1)) &&
                   !IsPassable(point + new Vector2Int(0, -1));
        }

        public char NumAlphaCode(int i)
        {
            if (i <= 9)
                return (char)('0' + i);
            return (char)('A' + i - 9);
        }

        public void WriteTextRepresentation(string filename = "test_map.txt")
        {
            var printMap = new char[W * 3][];
            for (var i = 0; i < W * 3; i++) printMap[i] = new char[H * 3];

            for (var x = 0; x < W; x++)
            for (var y = 0; y < H; y++)
            {
                for (var dx = 0; dx <= 2; dx++)
                for (var dy = 0; dy <= 2; dy++)
                    printMap[x * 3 + dx][y * 3 + dy] = '#';

                printMap[x * 3 + 1][y * 3 + 1] = geometry[x][y] == 0 ? '#' : NumAlphaCode(geometry[x][y]);
                // add the edges
                if (edges[x][y] != Edges.None)
                {
                    printMap[x * 3 + 1][y * 3 + 2] = edges[x][y].HasFlag(Edges.Top) ? '|' : ' ';
                    printMap[x * 3 + 1][y * 3] = edges[x][y].HasFlag(Edges.Bottom) ? '|' : ' ';
                    printMap[x * 3][y * 3 + 1] = edges[x][y].HasFlag(Edges.Left) ? '-' : ' ';
                    printMap[x * 3 + 2][y * 3 + 1] = edges[x][y].HasFlag(Edges.Right) ? '-' : ' ';
                }
            }

            using var file = new StreamWriter($"Assets/Resources/{filename}");
            for (var y = 0; y < H * 3; y++)
            {
                for (var x = 0; x < W * 3; x++) file.Write(printMap[x][W * 3 - y - 1]);
                file.WriteLine();
            }

            Debug.Log("Wrote contents of map to " + filename);
        }

        public IEnumerable<(Vector2Int, int)> Neighborhood(Vector2Int origin)
        {
            // int count = 0;
            for (var dx = -1; dx <= 1; dx++)
            for (var dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                if (dx * dy != 0) // diagonal
                    continue;

                if (IsPointValid(origin + new Vector2Int(dx, dy)))
                    yield return (origin + new Vector2Int(dx, dy), geometry[origin.x + dx][origin.y + dy]);
            }
        }

        public IEnumerable<Vector2Int> NeighborsPositions(Vector2Int origin)
        {
            // int count = 0;
            return Neighborhood(origin).Where(it => it.Item2 != 0).Select(it => it.Item1);
        }

        public IEnumerable<Vector2Int> EmptySpacePositions(Vector2Int origin)
        {
            return Neighborhood(origin).Where(it => it.Item2 == 0).Select(it => it.Item1);
        }

        private void ConnectAdjacent(Vector2Int src, Vector2Int target)
        {
            if (src == target) return;

            var diff = target - src;
            var edge = FromDir(diff);
            edges[src.x][src.y] |= edge;
            edges[target.x][target.y] |= Opposite(edge);
        }

        private void ConnectAllNeighbors(Vector2Int point)
        {
            foreach (var neighbor in NeighborsPositions(point)) ConnectAdjacent(point, neighbor);
        }

        public int OccupiedNeighbors(Vector2Int origin)
        {
            return NeighborsPositions(origin).Sum(p => 1);
        }

        // find a point in the map that has the least connected neighbors with stochaticity
        public Vector2Int SampleStartingPoint()
        {
            var bestScore = float.NegativeInfinity;
            var bestPoint = new Vector2Int(-1, -1);
            foreach (var mapNode in nodes)
            {
                if (mapNode == null) continue;
                var score = (4 - OccupiedNeighbors(mapNode.position)) * Random.Range(0.75f, 1.25f);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPoint = EmptySpacePositions(mapNode.position).First();
                }
            }

            return bestPoint;
        }

        private Vector2Int RandomDir()
        {
            var i = 0;
            i = Random.Range(0, 4);
            if (i == 0) return new Vector2Int(0, -1);
            if (i == 1) return new Vector2Int(0, 1);
            if (i == 2) return new Vector2Int(-1, 0);
            if (i == 3) return new Vector2Int(1, 0);
            throw new Exception("RandomDir() failed");
        }

        // start a random walk at a certain point, creates a node at that point
        private void RandomWalk(Vector2Int startPoint, int ttNodes, bool start = false)
        {
            if (!IsPointValid(startPoint))
            {
                Debug.LogWarning("Invalid start point");
                return;
            }

            var dir = RandomDir();
            var curPoint = startPoint;
            geometry[curPoint.x][curPoint.y] = nodes.Count;
            if (start)
                nodes.Add(new StartNode(nodes.Count, curPoint));
            else
                nodes.Add(new TestMapNode(nodes.Count, curPoint));
            while (ttNodes > 0)
            {
                if (IsStuck(curPoint)) break;
                dir = markovDigger.state switch
                {
                    MarkovDigger.State.DigLeft => RotateLeft(dir),
                    MarkovDigger.State.DigRight => RotateRight(dir),
                    _ => dir
                };

                var nextPoint = curPoint + dir;
                var triesLeft = 100;
                while ((!IsPointValid(nextPoint) || !IsPassable(nextPoint)) && triesLeft > 0)
                {
                    markovDigger.ProgressState();
                    dir = markovDigger.state switch
                    {
                        MarkovDigger.State.DigLeft => RotateLeft(dir),
                        MarkovDigger.State.DigRight => RotateRight(dir),
                        _ => dir
                    };
                    nextPoint = curPoint + dir;
                    triesLeft--;
                }

                if (triesLeft == 0)
                {
                    Debug.Log("no tries left");
                    break;
                }

                edges[curPoint.x][curPoint.y] |= FromDir(dir);
                edges[nextPoint.x][nextPoint.y] |= Opposite(FromDir(dir));
                geometry[nextPoint.x][nextPoint.y] = nodes.Count;
                Debug.Log($"Added node {nodes.Count} at {nextPoint}");
                nodes.Add(new TestMapNode(nodes.Count, nextPoint));
                curPoint = nextPoint;
                // maintenance
                ttNodes--;
            }
        }

        public void Generate()
        {
            // we do a random walk based thing
            var centerW = W / 2;
            var centerH = H / 2;
            // int ttNodes = 30;
            startPoint = new Vector2Int(centerW, centerH);

            RandomWalk(startPoint, 5, true);
            for (var i = 0; i < 3; i++)
            {
                var nextPoint = SampleStartingPoint();
                RandomWalk(nextPoint, 5);
                ConnectAllNeighbors(nextPoint);
            }
        }

        public IEnumerable<MapNode> Dfs()
        {
            var stack = new Stack<MapNode>();
            var visited = new HashSet<int>();
            stack.Push(nodes[1]);
            visited.Add(1);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                yield return node;
                foreach (var (pos, nodeId) in Neighborhood(node.position))
                {
                    if (nodeId == 0) continue;
                    if (visited.Contains(nodeId)) continue;
                    visited.Add(nodeId);
                    stack.Push(nodes[nodeId]);
                }
            }
        }

        public bool HasEdge(Vector2Int u, Vector2Int v)
        {
            var diff = v - u;
            switch ((diff.x, diff.y))
            {
                case (0, 1):
                    return (edges[u.x][u.y] & Edges.Top) != 0;
                case (0, -1):
                    return (edges[u.x][u.y] & Edges.Bottom) != 0;
                case (1, 0):
                    return (edges[u.x][u.y] & Edges.Right) != 0;
                case (-1, 0):
                    return (edges[u.x][u.y] & Edges.Left) != 0;
                default:
                    return false;
            }
        }

        public IEnumerable<(MapNode, MapNode)> EachEdge()
        {
            var visited = new HashSet<(int, int)>();
            for (var i = 1; i < nodes.Count; i++)
            {
                var node = nodes[i];
                foreach (var (pos, nodeId) in Neighborhood(node.position))
                {
                    if (nodeId == 0) continue;
                    if (nodeId < i) continue;
                    if (visited.Contains((i, nodeId))) continue;
                    if (!HasEdge(node.position, pos)) continue;
                    visited.Add((i, nodeId));
                    yield return (node, nodes[nodeId]);
                }
            }
        }
    }
}