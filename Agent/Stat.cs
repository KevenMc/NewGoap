using System;
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
        public Item item;
        public Blueprint blueprint;
        public Station station;
        public Agent agent;
        public Stat delegateGoal;
        public Vector3 location;
        public StationSO stationData;
        public Inventory inventory;
        public float current;
        public float trigger;
        public float increment;
        public int priority;
        public Boolean isUrgent = false;

        public Stat()
        {
            this.statType = StatType.Null;
        }

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
            this.isUrgent = stat.isUrgent;
            this.itemData = stat.itemData;
        }

        public Stat(StatType statType, ItemSO itemData)
        {
            this.statType = statType;
            this.itemData = itemData;
        }

        public Stat(StatType statType, Item item)
        {
            this.statType = statType;
            this.itemData = item.itemData;
            this.item = item;
            this.location = item.location;
        }

        public Stat(StatType statType, Station station)
        {
            this.statType = statType;
            this.station = station;
            this.stationData = station.stationData;
            this.location = station.location;
        }

        public Stat(StatType statType, StationSO stationData)
        {
            this.statType = statType;
            this.stationData = stationData;
        }

        public Stat(StatType statType, Blueprint blueprint)
        {
            this.statType = statType;
            this.blueprint = blueprint;
            this.itemData = blueprint.craftedItem;
            this.stationData = blueprint.craftingStation;
        }

        public Stat(StatType statType, Agent agent, Stat delegateGoal)
        {
            this.statType = statType;
            this.agent = agent;
            this.delegateGoal = delegateGoal;
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
            string retString = "Satisfy : " + statType.ToString() + " : ";
            if (itemData != null)
                retString += "Item type : " + itemData.itemName;
            if (stationData != null)
            {
                retString += "Station type : " + stationData;
            }
            if (station != null)
            {
                retString += "Station : " + station;
            }
            return retString;
        }

        public Boolean IsUrgent()
        {
            return isUrgent;
        }
    }
}
