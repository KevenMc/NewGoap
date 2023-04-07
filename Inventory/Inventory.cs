using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class Inventory : MonoBehaviour
    {
        public Agent agent;

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
            // AddItem(defaultItem);
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

        public void UseItem(ItemSO item, Agent useAgent)
        {
            var inventoryItem = items.Find(x => x.item == item);

            if (inventoryItem != null && inventoryItem.quantity > 0)
            {
                foreach (var statEffect in item.statEffects)
                {
                    useAgent.statHandler.ModifyStat(statEffect.statType, statEffect.value);
                }

                RemoveItem(item);
            }
        }
    }
}
