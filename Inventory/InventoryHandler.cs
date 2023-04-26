using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP
{
    public class InventoryHandler : MonoBehaviour
    {
        public void Init() { }

        public List<ItemSO> ReturnGoalItems(Stat goal, Inventory inventory)
        {
            if (inventory.items == null)
            {
                Debug.LogWarning("Inventory items list is null!");
                return new List<ItemSO>();
            }

            switch (goal.statType)
            {
                case StatType.Have_Item_In_Inventory:
                case StatType.Have_Item_Equipped:
                    return inventory.items
                        .Where(inventoryItem => inventoryItem.itemData == goal.itemData)
                        .Select(inventoryItem => inventoryItem.itemData)
                        .ToList();

                default:
                    Debug.Log(goal.statType);
                    return inventory.items
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

        public void AddItem(ItemSO itemData, Inventory inventory, int quantity = 1)
        {
            Debug.Log("Adding item to inventory : " + itemData.itemName);
            var inventoryItem = inventory.items.Find(x => x.itemData == itemData);

            if (inventoryItem != null)
            {
                inventoryItem.quantity += quantity;
            }
            else
            {
                inventory.items.Add(
                    new Inventory.InventoryItem { itemData = itemData, quantity = quantity }
                );
            }
        }

        public void RemoveItem(ItemSO itemData, Inventory inventory, int quantity = 1)
        {
            var inventoryItem = inventory.items.Find(x => x.itemData == itemData);

            if (inventoryItem != null)
            {
                inventoryItem.quantity -= quantity;

                if (inventoryItem.quantity <= 0)
                {
                    inventory.items.Remove(inventoryItem);
                }
            }
        }

        public void UseItem(ItemSO itemData, Inventory inventory, Agent useAgent)
        {
            Debug.Log("Using an item " + itemData.itemName);
            var inventoryItem = inventory.items.Find(x => x.itemData == itemData);

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

                RemoveItem(itemData, inventory);
            }
        }
    }
}
