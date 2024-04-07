using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class Inventory : MonoBehaviour
    {
        [System.Serializable]
        public class InventoryItem
        {
            public ItemSO itemData;
            public int quantity;
            public List<StatEffect> statEffects;
        }

        public List<InventoryItem> items = new List<InventoryItem>();

        private void Start()
        {
            Debug.Log(items.Count);
            foreach (InventoryItem inventoryItem in items)
            {
                Debug.Log("***********************************");
                Debug.Log(inventoryItem.itemData.itemName);
                inventoryItem.statEffects.AddRange(inventoryItem.itemData.statEffects);
            }
        }
    }
}
