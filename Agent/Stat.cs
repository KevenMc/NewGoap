using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    [System.Serializable]
    public class Stat
    {
        public StatType statType;
        public ItemSO itemData;
        public float current;
        public float trigger;
        public float increment;
        public int priority;

        public Stat(StatType statType, float current, float trigger, float increment, int priority)
        {
            this.statType = statType;
            this.current = current;
            this.trigger = trigger;
            this.increment = increment;
            this.priority = priority;
        }

        public Stat(Stat stat)
        {
            this.statType = stat.statType;
            this.current = stat.current;
            this.trigger = stat.trigger;
            this.increment = stat.increment;
            this.priority = stat.priority;
        }

        public Stat(StatType statType, ItemSO itemData)
        {
            this.statType = statType;
            this.itemData = itemData;
        }

        public Stat(
            StatType statType,
            ItemSO itemData,
            float current,
            float trigger,
            float increment,
            int priority
        )
        {
            this.statType = statType;
            this.itemData = itemData;
            this.current = current;
            this.trigger = trigger;
            this.increment = increment;
            this.priority = priority;
        }

        public override string ToString()
        {
            string retString = "StatType : " + statType.ToString() + "\n";
            if (itemData != null)
                retString += "Item type : " + itemData.itemName;
            return retString;
        }
    }
}
