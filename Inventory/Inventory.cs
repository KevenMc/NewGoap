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
            foreach (InventoryItem inventoryItem in items)
            {
                inventoryItem.statEffects.AddRange(inventoryItem.itemData.statEffects);
            }
        }
    }
}
