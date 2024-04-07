using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    [CreateAssetMenu(fileName = "New Blueprint", menuName = "GOAP/Crafting/Blueprint/Blueprint")]
    public class Blueprint : ScriptableObject
    {
        public string blueprintName;
        public string description;
        public float actionCost;
        public List<ItemRequirement> requiredItems;
        public ItemSO requiredTool;
        public StationSO craftingStation;
        public ItemSO craftedItem;

        [System.Serializable]
        public class ItemRequirement
        {
            public ItemSO itemData;
            public bool destroyOnCraft;
        }
    }
}
