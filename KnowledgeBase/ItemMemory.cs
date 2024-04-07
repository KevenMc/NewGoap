using System.Collections.Generic;
using UnityEngine;
using System;

namespace GOAP
{
    public class ItemMemory
    {
        public Dictionary<ItemSO, List<Item>> itemLocations = new Dictionary<ItemSO, List<Item>>();
        public Dictionary<StatType, List<Item>> itemsByStat =
            new Dictionary<StatType, List<Item>>();

        public ItemMemory()
        {
            itemLocations = new Dictionary<ItemSO, List<Item>>();
            itemsByStat = new Dictionary<StatType, List<Item>>();
        }

        public void AddItem(Item item)
        {
            ItemSO itemData = item.itemData;
            if (!itemLocations.ContainsKey(itemData))
            {
                itemLocations[itemData] = new List<Item>();
            }
            itemLocations[itemData].Add(item);

            foreach (StatEffect statEffect in itemData.statEffects)
            {
                StatType statType = statEffect.statType;
                if (!itemsByStat.ContainsKey(statType))
                {
                    itemsByStat[statType] = new List<Item>();
                }
                itemsByStat[statType].Add(item);
            }
        }

        public void RemoveItem(Item item)
        {
            ItemSO itemData = item.itemData;

            // Remove item from itemLocations dictionary
            if (itemLocations.ContainsKey(itemData))
            {
                itemLocations[itemData].Remove(item);
                // If the list is empty after removing the item, remove the key from the dictionary
                if (itemLocations[itemData].Count == 0)
                {
                    itemLocations.Remove(itemData);
                }
            }

            // Remove item from itemsByStat dictionary
            foreach (StatEffect statEffect in itemData.statEffects)
            {
                StatType statType = statEffect.statType;
                if (itemsByStat.ContainsKey(statType))
                {
                    itemsByStat[statType].Remove(item);
                    // If the list is empty after removing the item, remove the key from the dictionary
                    if (itemsByStat[statType].Count == 0)
                    {
                        itemsByStat.Remove(statType);
                    }
                }
            }
        }

        public List<Item> ItemsByStat(Stat goal)
        {
            List<Item> items = new List<Item>();

            if (itemsByStat.ContainsKey(goal.statType))
            {
                items.AddRange(itemsByStat[goal.statType]);
            }

            return items;
        }

        public List<Item> ItemsByItem(Stat goal)
        {
            List<Item> items = new List<Item>();

            if (itemLocations.ContainsKey(goal.itemData))
            {
                items.AddRange(itemLocations[goal.itemData]);
            }

            return items;
        }
    }
}
