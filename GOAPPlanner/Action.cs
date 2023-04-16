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
        public Boolean isOwnMaster = false;
        public bool hasMasterAction = true;
        public Action parentAction = null;
        public List<Action> childActions = new List<Action>();
        public List<Action> subActions = new List<Action>();
        public bool canComplete = false;
        public Boolean isSubAction = false;
        public ActionStatus actionStatus = ActionStatus.WaitingToExecute;
        public AnimationClip animation;
        #endregion

        #region
        public float actionCost = 1;
        public ItemSO itemData;
        public StationSO stationData;
        public Item item;
        public Vector3 location;
        public Blueprint blueprint;
        #endregion

        public Action(Stat goal)
        {
            this.goal = goal;
            this.hasMasterAction = false;
            this.isOwnMaster = true;
            this.masterAction = this;
            this.actionName = goal.ToString();
            Debug.Log("Master action is : " + this.masterAction.actionName);
        }

        public Action(Action parentAction, Boolean isSubAction = false)
        {
            this.parentAction = parentAction;
            if (isSubAction)
            {
                this.masterAction = parentAction;
                parentAction.subActions.Add(this);
            }
            else
            {
                this.masterAction = parentAction.masterAction;
                parentAction.childActions.Add(this);
            }
        }

        // Init method to use item from inventory
        public void Init(
            ActionType actionType,
            Stat goal,
            ItemSO itemData,
            Boolean canComplete = false
        )
        {
            Debug.Log("Init for item from inventory");
            this.goal = goal;
            this.actionType = actionType;
            this.itemData = itemData;
            this.actionName = actionType.ToString() + " : " + itemData.itemName;
            this.canComplete = canComplete;
            this.actionCost += this.parentAction.actionCost + itemData.itemUseCost;
        }

        // Init method to use STATION
        public void Init(
            ActionType actionType,
            Stat goal,
            StationSO stationData,
            Boolean canComplete = false
        )
        {
            Debug.Log("Init for station");
            this.goal = goal;
            this.actionType = actionType;
            this.stationData = stationData;
            this.actionName = actionType.ToString() + " : " + stationData.stationName;
            this.canComplete = canComplete;
            this.actionCost += this.parentAction.actionCost;
        }

        // Init method to collect item
        public void Init(ActionType actionType, Stat goal, Item item, Boolean canComplete = false)
        {
            this.goal = goal;
            Debug.Log("Init for collect item");
            this.actionType = actionType;
            this.goal = goal;
            this.item = item;
            this.actionName = actionType.ToString() + " : " + item.itemData.itemName;
            this.canComplete = canComplete;
            this.actionCost += this.parentAction.actionCost;
        }

        // Init method to move to location
        public void Init(
            ActionType actionType,
            Stat goal,
            Vector3 location,
            Boolean canComplete = false
        )
        {
            Debug.Log("Init blueprint for location");
            this.goal = goal;
            this.actionType = actionType;
            this.location = location;
            this.actionName = actionType.ToString() + " : " + location;
            this.canComplete = canComplete;
            this.actionCost += this.parentAction.actionCost;
        }

        // Init method for blueprint
        public void Init(ActionType actionType, Blueprint blueprint, Boolean canComplete = false)
        {
            Debug.Log("Init for blueprint");
            this.actionType = actionType;
            this.blueprint = blueprint;
            this.stationData = blueprint.craftingStation;
            this.actionName = actionType.ToString() + " : " + blueprint.blueprintName;
            this.canComplete = canComplete;
            this.actionCost += this.parentAction.actionCost;
            Debug.Log(this.actionName);
        }

        // Init method for blueprint sub-actions
        public void Init(
            ActionType actionType,
            ItemSO itemData,
            Action parentAction,
            Boolean canComplete = false
        )
        {
            Debug.Log("Init for blueprint sub-action");
            this.actionType = actionType;
            this.itemData = itemData;
            this.actionName = actionType.ToString() + " : " + itemData.itemName;
            this.parentAction = parentAction;
            this.masterAction = this;
            this.hasMasterAction = false;
            this.isOwnMaster = true;
            this.canComplete = canComplete;
            this.actionCost += this.parentAction.actionCost;
        }

        public Boolean CanComplete()
        {
            foreach (Action subAction in subActions)
            {
                if (!subAction.canComplete)
                {
                    return false;
                }
            }
            if (subActions.Count > 0)
            {
                canComplete = true;
            }
            return canComplete;
        }
    }
}
