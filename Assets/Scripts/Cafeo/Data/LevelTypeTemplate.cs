using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.Data
{
    public class LevelTypeTemplate : ScriptableObject
    {
        public string levelName;
        public int minFloors;
        public int maxFloors;

        public List<string> treasurePool;
        public List<string> enemyPool;
        public List<string> shopPool;
    }
}