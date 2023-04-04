using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class Inventory : MonoBehaviour
    {
        [System.Serializable]
        public class InventoryItem
        {
            public ItemSO item;
            public int quantity;
            public List<StatEffect> statEffects;
        }

        public ItemSO defaultItem;

        public List<InventoryItem> items = new List<InventoryItem>();

        void Start()
        {
            AddItem(defaultItem);
        }

        public List<ItemSO> returnGoalItemsx(Stat goal)
        {
            Debug.Log("Return goal items");
            Debug.Log(goal.statType.ToString());
            foreach (InventoryItem invitem in items)
            {
                Debug.Log(invitem.item.itemName);
            }
            List<ItemSO> goalItems = items
                .Where(
                    inventoryItem =>
                        inventoryItem.statEffects.Any(
                            statEffect => statEffect.statType == goal.statType
                        )
                )
                .Select(inventoryItem => inventoryItem.item)
                .ToList();

            Debug.Log(goalItems[0]);
            return goalItems;
        }

        public List<ItemSO> returnGoalItems(Stat goal)
        {
            if (items == null)
            {
                Debug.LogWarning("Inventory items list is null!");
                return new List<ItemSO>();
            }

            return items
                .Where(
                    inventoryItem =>
                        inventoryItem.item.statEffects.Any(
                            statEffect => statEffect.statType == goal.statType
                        )
                )
                .Select(inventoryItem => inventoryItem.item)
                .ToList();
        }

        public void AddItem(ItemSO item, int quantity = 1)
        {
            var inventoryItem = items.Find(x => x.item == item);

            if (inventoryItem != null)
            {
                inventoryItem.quantity += quantity;
            }
            else
            {
                items.Add(new InventoryItem { item = item, quantity = quantity });
            }
        }

        public void RemoveItem(ItemSO item, int quantity = 1)
        {
            var inventoryItem = items.Find(x => x.item == item);

            if (inventoryItem != null)
            {
                inventoryItem.quantity -= quantity;

                if (inventoryItem.quantity <= 0)
                {
                    items.Remove(inventoryItem);
                }
            }
        }

        public void UseItem(ItemSO item, Agent agent)
        {
            var inventoryItem = items.Find(x => x.item == item);

            if (inventoryItem != null && inventoryItem.quantity > 0)
            {
                foreach (var statEffect in item.statEffects)
                {
                    agent.statHandler.ModifyStat(statEffect.statType, statEffect.value);
                }

                RemoveItem(item);
            }
        }
    }
}
