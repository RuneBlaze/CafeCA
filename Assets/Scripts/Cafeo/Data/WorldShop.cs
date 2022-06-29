using System;
using System.Collections.Generic;

namespace Cafeo.Data
{
    [Serializable]
    public class WorldShop
    {
        public string displayName;
        public Inventory inventory;
        public int gold;
        public WorldItem.ItemPool itemPool;

        private void Init()
        {
            inventory = new Inventory();
        }

        public void TryImportGoods()
        {
            
        }
    }
}