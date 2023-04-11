using System.Collections.Generic;
using UnityEngine;

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

            foreach (ItemSO.StatEffect statEffect in itemData.statEffects)
            {
                StatType statType = statEffect.statType;
                if (!itemsByStat.ContainsKey(statType))
                {
                    itemsByStat[statType] = new List<Item>();
                }
                itemsByStat[statType].Add(item);
            }
        }

        public List<Item> returnGoalItems(Stat goal)
        {
            StatType goalStatType = goal.statType;
            List<Item> items = new List<Item>();

            switch (goalStatType)
            {
                case StatType.Have_Item_Equipped:
                    if (itemLocations.ContainsKey(goal.itemData))
                    {
                        items.AddRange(itemLocations[goal.itemData]);
                    }
                    break;
                default:

                    if (itemsByStat.ContainsKey(goalStatType))
                    {
                        items.AddRange(itemsByStat[goalStatType]);
                    }

                    break;
            }

            return items;
        }

        // public List<Item> GetItemLocations(StatType statType)
        // {
        //     List<Item> items = new List<Item>();
        //     if (itemsByStat.ContainsKey(statType))
        //     {
        //         items.AddRange(itemsByStat[statType]);
        //     }
        //     return items;
        // }
    }
}
