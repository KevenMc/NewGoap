using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class InventoryHandler : MonoBehaviour
    {
        // public Agent agent;

        [System.Serializable]
        public class InventoryItem
        {
            public ItemSO itemData;
            public int quantity;
            public List<StatEffect> statEffects;
        }

        public List<InventoryItem> items = new List<InventoryItem>();

        public void Init() { }

        public List<ItemSO> ReturnGoalItems(Stat goal)
        {
            if (items == null)
            {
                Debug.LogWarning("Inventory items list is null!");
                return new List<ItemSO>();
            }
            if (goal.statType == StatType.Have_Item_In_Inventory)
            {
                return items
                    .Where(inventoryItem => inventoryItem.itemData == goal.itemData)
                    .Select(inventoryItem => inventoryItem.itemData)
                    .ToList();
            }
            else
            {
                return items
                    .Where(
                        inventoryItem =>
                            inventoryItem.itemData.statEffects.Any(
                                statEffect => statEffect.statType == goal.statType
                            )
                    )
                    .Select(inventoryItem => inventoryItem.itemData)
                    .ToList();
            }
        }

        public void AddItem(ItemSO itemData, int quantity = 1)
        {
            Debug.Log("Adding item to inventory : " + itemData.itemName);
            var inventoryItem = items.Find(x => x.itemData == itemData);

            if (inventoryItem != null)
            {
                inventoryItem.quantity += quantity;
            }
            else
            {
                items.Add(new InventoryItem { itemData = itemData, quantity = quantity });
            }
        }

        public void RemoveItem(ItemSO itemData, int quantity = 1)
        {
            var inventoryItem = items.Find(x => x.itemData == itemData);

            if (inventoryItem != null)
            {
                inventoryItem.quantity -= quantity;

                if (inventoryItem.quantity <= 0)
                {
                    items.Remove(inventoryItem);
                }
            }
        }

        public void UseItem(ItemSO itemData, Agent useAgent)
        {
            Debug.Log("Using an item " + itemData.itemName);
            var inventoryItem = items.Find(x => x.itemData == itemData);

            if (inventoryItem != null && inventoryItem.quantity > 0)
            {
                if (!StatManager.instance.statsToModify.ContainsKey(useAgent.statHandler))
                {
                    StatManager.instance.statsToModify[useAgent.statHandler] =
                        new List<StatEffect>();
                }
                foreach (StatEffect statEffect in itemData.statEffects)
                {
                    StatManager.instance.statsToModify[useAgent.statHandler].Add(statEffect);
                }
                // foreach (var statEffect in itemData.statEffects)
                // {
                //     useAgent.statHandler.ModifyStat(statEffect.statType, statEffect.value);
                // }

                RemoveItem(itemData);
            }
        }
    }
}
