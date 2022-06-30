using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Data
{
    public class WorldItem
    {
        public Sprite icon;
        public string displayName;
        public int basePrice;
        public string tooltip;
        public bool hasMaterial;

        [System.Flags]
        public enum ItemPool
        {
            Konbini = 1,
            Oriental = 2,
            Grocery = 4,
        }
    }
}