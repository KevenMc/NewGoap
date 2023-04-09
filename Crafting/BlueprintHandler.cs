using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class BlueprintHandler : MonoBehaviour
    {
        public Inventory inventory;

        public void Init() { }

        public void CompleteBlueprintNoStation(Blueprint blueprint)
        {
            foreach (Blueprint.ItemRequirement item in blueprint.requiredItems)
            {
                if (item.destroyOnCraft)
                {
                    inventory.RemoveItem(item.itemData);
                }
            }
            inventory.AddItem(blueprint.craftedItem);
        }
    }
}
