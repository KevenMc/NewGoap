using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class KnowledgeHandler : MonoBehaviour
    {
        public Item itemToAdd;
        public ItemMemory itemMemory = new ItemMemory();

        void Start()
        {
            itemMemory = new ItemMemory();
            itemMemory.AddItem(itemToAdd);
            LogMemoryContents();
        }

        void LogMemoryContents()
        {
            Debug.Log("Item Memory Contents:");
            foreach (var item in itemMemory.itemLocations)
            {
                Debug.Log("Item: " + item);
            }
        }
    }
}
