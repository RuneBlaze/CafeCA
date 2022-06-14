using System;
using System.Collections.Generic;

namespace Cafeo.Data
{
    [Serializable]
    public class DropInventory
    {
        public int coins;
        public int keys;
        public List<Charm> charms;
        public List<OneTimeUseItem> oneTimeUseItems;
        public List<Treasure> treasures;

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


        public static DropInventory OnlyCoins(int coins)
        {
            var inventory = new DropInventory
            {
                coins = coins
            };
            return inventory;
        }

        public static DropInventory OnlyKeys(int keys)
        {
            var inventory = new DropInventory
            {
                keys = keys
            };
            return inventory;
        }

        public static DropInventory CoinsAndKeys(int coins, int keys)
        {
            var inventory = new DropInventory
            {
                coins = coins,
                keys = keys
            };
            return inventory;
        }

        public static DropInventory OneTimeUseItems(List<OneTimeUseItem> items)
        {
            var inventory = new DropInventory
            {
                oneTimeUseItems = items
            };
            return inventory;
        }

        public static DropInventory SingleItem(OneTimeUseItem item)
        {
            var inventory = new DropInventory
            {
                oneTimeUseItems = new List<OneTimeUseItem> { item }
            };
            return inventory;
        }

        public static DropInventory operator +(DropInventory lhs, DropInventory rhs)
        {
            var inventory = new DropInventory
            {
                coins = lhs.coins + rhs.coins,
                keys = lhs.keys + rhs.keys
            };
            inventory.oneTimeUseItems.AddRange(lhs.oneTimeUseItems);
            inventory.oneTimeUseItems.AddRange(rhs.oneTimeUseItems);
            inventory.treasures.AddRange(lhs.treasures);
            inventory.treasures.AddRange(rhs.treasures);
            inventory.charms.AddRange(lhs.charms);
            inventory.charms.AddRange(rhs.charms);
            return inventory;
        }
    }
}