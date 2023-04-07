using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    [System.Serializable]
    public class Stat
    {
        public StatType statType;
        public ItemSO item;
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

        public Stat(
            StatType statType,
            ItemSO item,
            float current,
            float trigger,
            float increment,
            int priority
        )
        {
            this.statType = statType;
            this.item = item;
            this.current = current;
            this.trigger = trigger;
            this.increment = increment;
            this.priority = priority;
        }
    }
}
