using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class KnowledgeHandler : MonoBehaviour
    {
        public List<Item> itemsToAdd; //this is only for testing
        public List<BlueprintCatalogue> blueprintCatalogues = new List<BlueprintCatalogue>(); //testing
        public ItemMemory itemMemory = new ItemMemory();
        public BlueprintRepertoire blueprintRepertoire = new BlueprintRepertoire();

        public void Init()
        {
            itemMemory = new ItemMemory(); //testing
            foreach (Item itemToAdd in itemsToAdd) //
            {
                itemMemory.AddItem(itemToAdd); //testing
            }

            blueprintRepertoire.AddBlueprintCatalogues(blueprintCatalogues);
        }
    }
}
