using System;
using System.Collections.Generic;
using Cafeo.Utils;
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

        protected override void Setup()
        {
            base.Setup();
            width = 8;
            height = 8;
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
        }

        public bool IsValid(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
        
        public IEnumerable<TownOuterNode> OuterNeighbors {
            get
            {
                // only cardinal directions
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
}