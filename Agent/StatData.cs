using System;
using UnityEngine;

namespace GOAP
{
    public enum StatType
    {
        Null,
        Satisfied,
        Have_Item_Equipped,
        Have_Item_In_Inventory,
        Item_Is_At_Station,
        Be_At_Station,
        Move_To_Location,
        Move_To_Station_Location,
        Use_Station,
        Delegate_Action,
        Interrupt_Agent,
        Return_Delegate_Action,
        Blueprint,
        Storage,
        Hunger = 100,
        Thirst = 101,
        Energy = 102
    }

    [Serializable]
    public class StatEffect
    {
        public StatType statType;
        public float value;

        public StatEffect(StatType statType, float value)
        {
            this.statType = statType;
            this.value = value;
        }

        // public static explicit operator StatEffect(ItemSO.StatEffect se)
        // {
        //     return new StatEffect(se.statType, se.value);
        // }
    }
}
