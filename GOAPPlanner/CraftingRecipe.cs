using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    
[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName;
    public string description;
    public List<ItemRequirement> requiredItems;
    public ItemSO craftedItem;

    [System.Serializable]
    public class ItemRequirement
    {
        public ItemSO item;
        public int amount;
        public bool destroyOnCraft;
    }
}
}

