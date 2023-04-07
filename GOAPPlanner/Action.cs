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
        public Vector3 location;
        public ActionStatus actionStatus = ActionStatus.WaitingToExecute;
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
            this.actionCost = actionCost;
            this.goal = goal;
            this.itemData = itemData;
            this.item = item;
            this.location = location;
            this.canComplete = canComplete;
        }
    }
}
