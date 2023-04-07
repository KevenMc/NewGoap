using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class KnowledgeHandler : MonoBehaviour
    {
        public List<Item> itemsToAdd;
        public ItemMemory itemMemory = new ItemMemory();
        public BlueprintRepertoire blueprintRepertoire = new BlueprintRepertoire();

        void Start()
        {
            itemMemory = new ItemMemory();
            foreach (Item itemToAdd in itemsToAdd)
            {
                itemMemory.AddItem(itemToAdd);
            }
        }
    }
}
