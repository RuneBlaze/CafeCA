using System;
using System.Collections.Generic;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo
{
    public class HeightData : Singleton<HeightData>
    {
        public TextAsset heightData;
        private Dictionary<int, (float, float)> _maleData;
        private Dictionary<int, (float, float)> _femaleData;

        protected override void Setup()
        {
            _maleData = new Dictionary<int, (float, float)>();
            _femaleData = new Dictionary<int, (float, float)>();
            var src = heightData.text;
            var root = SimpleJSON.JSON.Parse(src);
            foreach (var key in root["boys"].Keys)
            {
                var packed = root["boys"][key];
                _maleData[int.Parse(key)] = (packed[0], packed[1]);
            }
            
            foreach (var key in root["girls"].Keys)
            {
                var packed = root["girls"][key];
                _femaleData[int.Parse(key)] = (packed[0], packed[1]);
            }
        }

        public void Start()
        {
            
        }

        public float GetHeight(AgentSoul.Gender gender, float age, float z)
        {
            if (_maleData == null)
            {
                Setup();
            }
            int months = Mathf.RoundToInt(Mathf.Clamp(age * 12, 61, 220));
            var (avg, std) = gender == AgentSoul.Gender.Male ? _maleData[months] : _femaleData[months];
            return avg + z * std;
        }
    }
}