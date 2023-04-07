using System;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    // public class Action
    // {
    //     public string actionName;
    //     public ActionType actionType;

    //     public int actionCost = 0;
    //     public Stat goal;
    //     public ItemSO itemData;

    //     public Item item;

    //     public Vector3 location;
    //     public ActionStatus actionStatus = ActionStatus.WaitingToExecute;
    //     public bool canComplete = false;

    //     // public List<StatEffect> statEffects = new List<StatEffect>();
    //     // public List<Plan> subPlans = new List<Plan>();

    //     public Action(ItemSO itemData, bool isComplete = false) //CCreate action from inventory
    //     {
    //         this.itemData = itemData;
    //         this.actionName = "Use " + itemData.itemName;
    //         this.actionType = ActionType.UseItem;
    //         this.canComplete = isComplete;
    //     }

    //     public Action(ItemSO itemData, Item item, bool isComplete = false) //create collect action
    //     {
    //         this.itemData = itemData;
    //         this.item = item;
    //         this.actionName = "Collect " + itemData.itemName;
    //         this.actionType = ActionType.CollectItem;
    //         this.canComplete = isComplete;
    //     }

    //     public Action(Vector3 location, string locationName, bool isComplete = false) //create action from location
    //     {
    //         this.location = location;
    //         this.actionName = "Move to " + locationName;
    //         this.actionType = ActionType.MoveToLocation;
    //         this.canComplete = isComplete;
    //     }

    //     public Action(Vector2 location, string locationName, bool isComplete = false) //create action from location
    //     {
    //         this.location = location;
    //         this.actionName = "Move to " + locationName;
    //         this.actionType = ActionType.MoveToLocation;
    //         this.canComplete = isComplete;
    //     }
    // }

    public class Action
    {
        public string actionName;
        public ActionType actionType;
        public float actionCost = 1;
        public Stat goal;
        public ItemSO itemData;
        public Item item;
        public Blueprint blueprint;
        public Vector3 location;
        public ActionStatus actionStatus = ActionStatus.WaitingToExecute;

        public List<Plan> subPlans = new List<Plan>();
        public Dictionary<Stat, List<Plan>> subPlanLists = new Dictionary<Stat, List<Plan>>();
        public bool canComplete = false;

        public Action(ActionType actionType)
        {
            this.actionType = actionType;
        }

        public void Setup(
            string actionName,
            float actionCost,
            Stat goal,
            ItemSO itemData,
            Item item,
            Vector3 location,
            bool canComplete
        )
        {
            this.actionName = actionName;
            //  this.actionCost = actionCost;
            this.goal = goal;
            this.itemData = itemData;
            this.item = item;
            this.location = location;
            this.canComplete = canComplete;
        }

        public void Setup(
            string actionName,
            float actionCost,
            Stat goal,
            ItemSO itemData,
            Blueprint blueprint,
            bool canComplete
        )
        {
            this.actionName = actionName;
            // this.actionCost = actionCost;
            this.goal = goal;
            this.itemData = itemData;
            this.blueprint = blueprint;
            this.canComplete = canComplete;
        }

        public float TotalActionCost()
        {
            float returnVal = actionCost;
            foreach (Plan subplan in subPlans)
            {
                returnVal += subplan.planCost;
            }
            return returnVal;
        }

        public void GenerateSubPlans()
        {
            if (actionType == ActionType.Blueprint)
            {
                Debug.Log("Blueprint : " + blueprint);
                foreach (Blueprint.ItemRequirement item in blueprint.requiredItems)
                {
                    Stat itemStat = new Stat(StatType.Item, item.item, 0, 0, 0, 1);
                    // Action collectAction = new Action(item, null, false);
                    Plan subPlan = new Plan(itemStat);
                    // subPlan.AddAction(collectAction);
                    if (!subPlanLists.ContainsKey(itemStat))
                    {
                        // If the key is not present, add it to the dictionary with a new list containing the value
                        subPlanLists.Add(itemStat, new List<Plan> { subPlan });
                    }
                    else
                    {
                        // If the key is present, append the value to the existing list
                        subPlanLists[itemStat].Add(subPlan);
                    }
                }
            }
        }
    }
}
