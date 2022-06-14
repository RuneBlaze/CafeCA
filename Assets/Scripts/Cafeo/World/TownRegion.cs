using System;
using System.Collections.Generic;
using Cafeo.Utils;
using Drawing;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Cafeo.World
{
    public class TownRegion : Singleton<TownRegion>
    {
        public TownOuterNode[,] outerNodes;
        public Transform worldRoot;
        public int width;
        public int height;
        public TownAgent player;
        public List<TownAgent> agents;
        public List<(Rect, Color)> miniMap;
        private Camera cam;

        protected override void Setup()
        {
            base.Setup();
            width = 8;
            height = 8;
            agents = new List<TownAgent>();
            miniMap = new List<(Rect, Color)>();
            cam = Camera.main;
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
            return go.AddComponent<TownOuterNode>();
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
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    outerNodes[x, y] = CreateOuterNode(x, y);
                    var dev = ChooseDeveloper(x, y);
                    dev.Furnish(outerNodes[x, y]);
                }
            }

            CreateMiniMap();
        }

        
        private const int SquareSize = 10;
        private void CreateMiniMap()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var n = outerNodes[x, y];
                    miniMap.Add((new Rect(x * SquareSize, y * SquareSize, SquareSize, SquareSize), n.color));
                }
            }
        }

        
        public void LateUpdate()
        {
            var draw = Draw.ingame;
            using (draw.InScreenSpace(cam))
            {
                foreach (var valueTuple in miniMap)
                {
                    var (rect, color) = valueTuple;
                    draw.SolidRectangle(rect, color);
                }
            }
        }

        public bool IsValid(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
        
        public IEnumerable<TownOuterNode> OuterNeighbors(TownOuterNode node) {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (x * x == y * y) continue;
                    if (!IsValid(x + width, y + height)) continue;
                    yield return outerNodes[x + width, y + height];
                }
            }
        }
    }
}