using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.Data
{
    [Serializable]
    public class DropInventory
    {
        public List<OneTimeUseItem> oneTimeUseItems;
        public List<Treasure> treasures;
        public List<Charm> charms;
        public int coins;
        public int keys;

        public DropInventory()
        {
            oneTimeUseItems = new List<OneTimeUseItem>();
            treasures = new List<Treasure>();
            charms = new List<Charm>();
            coins = 0;
            keys = 0;
        }
        
        public void AddOneTimeUseItem(OneTimeUseItem item)
        {
            oneTimeUseItems.Add(item);
        }
        
        public void AddTreasure(Treasure treasure)
        {
            treasures.Add(treasure);
        }
        
        public void AddCharm(Charm charm)
        {
            charms.Add(charm);
        }

        public void Explode(Vector2 center)
        {
            
        }
    }
}