using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class Action
    {
        #region
        public string actionName;
        public Stat goal; //this is the goal that is being satisfied by completing the immediate child action
        public ActionType actionType;
        public Action masterAction;
        public bool hasMasterAction = true;
        public Action parentAction;
        public List<Action> childActions = new List<Action>();
        public float actionCost = 1;
        public bool canComplete = false;
        public ActionStatus actionStatus = ActionStatus.WaitingToExecute;
        public List<Action> subActions = new List<Action>();
        #endregion

        #region
        public ItemSO itemData;
        public Item item;
        public Vector3 location;
        public Blueprint blueprint;
        #endregion

        public Action(Stat goal)
        {
            this.goal = goal;
            this.hasMasterAction = false;
            this.masterAction = this;
        }

        public Action(Action parentAction)
        {
            this.parentAction = parentAction;
            this.masterAction = parentAction.masterAction;
            parentAction.childActions.Add(this);
            this.actionCost += parentAction.actionCost;
        }

        // Init method to use item from inventory
        public void Init(
            ActionType actionType,
            Stat goal,
            ItemSO itemData,
            Boolean canComplete = false
        )
        {
            this.actionType = actionType;
            this.itemData = itemData;
            this.actionName = actionType.ToString() + " : " + itemData.itemName;
            this.canComplete = canComplete;
        }

        // Init method to collect item
        public void Init(ActionType actionType, Stat goal, Item item, Boolean canComplete = false)
        {
            this.actionType = actionType;
            this.item = item;
            this.actionName = actionType.ToString() + " : " + item.itemData.itemName;
            this.canComplete = canComplete;
        }

        // Init method to move to location
        public void Init(
            ActionType actionType,
            Stat goal,
            Vector3 location,
            Boolean canComplete = false
        )
        {
            this.actionType = actionType;
            this.location = location;
            this.actionName = actionType.ToString() + " : " + location;
            this.canComplete = canComplete;
        }

        // Init method for blueprint
        public void Init(ActionType actionType, Blueprint blueprint, Boolean canComplete = false)
        {
            this.actionType = actionType;
            this.blueprint = blueprint;
            this.actionName = actionType.ToString() + " : " + blueprint;
            this.canComplete = canComplete;
        }

        // Init method for blueprint sub-actions
        public void Init(ActionType actionType, ItemSO itemData, Boolean canComplete = false)
        {
            this.actionType = actionType;
            this.itemData = itemData;
            this.actionName = actionType.ToString() + " : " + itemData.itemName;
            this.canComplete = canComplete;
        }
    }
}
