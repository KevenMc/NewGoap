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

        public List<Item> ReturnGoalItemsByStat(Stat goal)
        {
            List<Item> items = new List<Item>();

            if (itemsByStat.ContainsKey(goal.statType))
            {
                items.AddRange(itemsByStat[goal.statType]);
            }

            return items;
        }

        public List<Item> ReturnGoalItemsByItem(Stat goal)
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
