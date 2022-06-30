using System;
using Cafeo.Fashion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cafeo.Data
{
    public class DebugInventory : Inventory
    {
        public enum DebugInventoryType
        {
            None,
            AllGarments,
        }
        
        [SerializeField] private DebugInventoryType debugInventoryType = DebugInventoryType.None;
        
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            PopulateInventory();
        }

        private void PopulateInventory()
        {
            if (debugInventoryType == DebugInventoryType.AllGarments)
            {
                PopulateFashionItems();
            }
            onDirty.Invoke();
        }

        private void PopulateFashionItems()
        {
            var fashion = FashionEngine.Instance;
            foreach (var series in fashion.CurrentSeasonCollection())
            {
                foreach (var wearable in series.Contents())
                {
                    if (!(Random.Range(0f, 1f) < 0.01f)) continue;
                    Add(wearable);
                }
            }
        }
    }
}