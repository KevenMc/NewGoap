using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    [CreateAssetMenu(fileName = "New Blueprint", menuName = "GOAP/Blueprint/Blueprint")]
    public class Blueprint : ScriptableObject
    {
        public string blueprintName;
        public string description;
        public List<ItemRequirement> requiredItems;
        public List<ItemRequirement> requiredTools;
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
