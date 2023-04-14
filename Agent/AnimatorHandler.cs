using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class AnimatorHandler : MonoBehaviour
    {
        public ActionHandler actionHandler;

        private void Start()
        {
            actionHandler = GetComponentInParent<ActionHandler>();
        }

        public void ExecuteAction()
        {
            actionHandler.ExecuteAction();
        }

        public void Collect_And_Equip()
        {
            actionHandler.Collect_And_Equip();
        }

        public void UnEquip_To_Inventory()
        {
            actionHandler.UnEquip_To_Inventory();
        }

        public void Make_Blueprint_From_Inventory()
        {
            actionHandler.Make_Blueprint_From_Inventory();
        }

        public void InstantiateEquippedItem()
        {
            actionHandler.InstantiateEquippedItem();
        }

        public void Use_Item()
        {
            actionHandler.Use_Item();
        }
    }
}
