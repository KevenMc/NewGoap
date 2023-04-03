using System.Collections.Generic;
using UnityEngine;

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

    public List<InventoryItem> items = new List<InventoryItem>();

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
