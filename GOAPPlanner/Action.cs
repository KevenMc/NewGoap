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
        public Action grandMasterAction;
        public Boolean isOwnMaster = false;
        public bool hasMasterAction = true;
        public Action parentAction = null;
        public List<Action> childActions = new List<Action>();
        public List<Action> subActions = new List<Action>();
        public bool canComplete = false;
        public Boolean isSubAction = false;
        public Boolean isLastSubAction = false;
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
            this.grandMasterAction = this;
            this.actionName = goal.ToString();

            Debug.Log("Master action is : " + this.masterAction.actionName);
            Debug.Log("action is : " + this.actionName);
        }

        public Action(Action parentAction, Boolean isSubAction = false)
        {
            this.parentAction = parentAction;
            this.actionName = actionType.ToString();

            this.grandMasterAction = parentAction.grandMasterAction;
            if (isSubAction)
            {
                Debug.Log("Adding subaction");
                this.masterAction = parentAction;
                parentAction.subActions.Add(this);
                Debug.Log(parentAction.subActions.Count);
                if (actionType == ActionType.Require_Move_To_Location)
                {
                    this.isLastSubAction = true;
                    Debug.Log(
                        "44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444"
                    );
                }
            }
            else
            {
                this.masterAction = parentAction.masterAction;
                parentAction.childActions.Add(this);
            }
        }

        // Init method to use item from inventory
        public void Init(ActionType actionType, Stat goal, Boolean canComplete = false)
        {
            // Debug.Log("Init for item from inventory");
            this.goal = goal;
            this.actionType = actionType;
            this.itemData = goal.itemData;
            this.item = goal.item;
            this.stationData = goal.stationData;
            this.location = goal.location;
            this.blueprint = goal.blueprint;
            switch (goal.itemData, goal.item, goal.stationData, goal.station, goal.blueprint)
            {
                case (_, null, null, null, null):
                    this.actionName = actionType.ToString() + " : " + goal.itemData;
                    break;

                case (null, _, null, null, null):
                    this.actionName = actionType.ToString() + " : " + goal.item;
                    break;
                case (null, null, _, null, null):
                    this.actionName = actionType.ToString() + " : " + goal.stationData;
                    break;
                case (null, null, null, _, null):
                    this.actionName = actionType.ToString() + " : " + goal.station;
                    break;
                case (null, null, null, null, _):
                    this.actionName = actionType.ToString() + " : " + goal.blueprint;
                    break;
                default:
                    this.actionName = actionType.ToString();
                    break;
            }

            this.canComplete = canComplete;
            if (actionType == ActionType.Require_Move_To_Location)
            {
                this.isLastSubAction = true;
                Debug.Log(
                    "44444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444"
                );
            }
            this.actionCost += this.parentAction.actionCost;
        }

        // // Init method to use STATION
        // public void Init(
        //     ActionType actionType,
        //     Stat goal,
        //     StationSO stationData,
        //     Boolean canComplete = false
        // )
        // {
        //     // Debug.Log("Init for station");
        //     this.goal = goal;
        //     this.actionType = actionType;
        //     this.stationData = stationData;
        //     this.actionName = actionType.ToString() + " : " + stationData.stationName;
        //     this.canComplete = canComplete;
        //     this.actionCost += this.parentAction.actionCost;
        // }

        // // Init method to collect item
        // public void Init(ActionType actionType, Stat goal, Item item, Boolean canComplete = false)
        // {
        //     this.goal = goal;
        //     // Debug.Log("Init for collect item");
        //     this.actionType = actionType;
        //     this.goal = goal;
        //     this.actionName = actionType.ToString() + " : " + item.itemData.itemName;
        //     this.canComplete = canComplete;
        //     this.actionCost += this.parentAction.actionCost;
        // }

        // // Init method to move to location
        // public void Init(
        //     ActionType actionType,
        //     Stat goal,
        //     Vector3 location,
        //     Boolean canComplete = false
        // )
        // {
        //     // Debug.Log("Init blueprint for location");
        //     this.goal = goal;
        //     this.actionType = actionType;
        //     this.actionName = actionType.ToString() + " : " + location;
        //     this.canComplete = canComplete;
        //     this.actionCost += this.parentAction.actionCost;
        // }

        // // Init method for blueprint
        // public void Init(ActionType actionType, Blueprint blueprint, Boolean canComplete = false)
        // {
        //     // Debug.Log("Init for blueprint");
        //     this.actionType = actionType;
        //     this.stationData = blueprint.craftingStation;
        //     if (actionType == ActionType.Move_To_Location)
        //     {
        //         this.actionName = actionType.ToString() + " : " + blueprint.blueprintName;
        //     }
        //     // else if (actionType == ActionType.Move_To_Station_Location)
        //     // {
        //     //     this.actionName = actionType.ToString() + " : " + blueprint.craftingStation;
        //     // }
        //     else
        //     {
        //         this.actionName = actionType.ToString();
        //     }
        //     this.canComplete = canComplete;
        //     this.actionCost += this.parentAction.actionCost;
        // }

        // // Init method for blueprint sub-actions
        // public void Init(
        //     ActionType actionType,
        //     ItemSO itemData,
        //     Action parentAction,
        //     Boolean canComplete = false
        // )
        // {
        //     // Debug.Log("Init for blueprint sub-action");
        //     this.actionType = actionType;
        //     this.itemData = itemData;
        //     this.actionName = actionType.ToString() + " : " + itemData.itemName;
        //     this.parentAction = parentAction;
        //     this.masterAction = this;
        //     this.hasMasterAction = false;
        //     this.isOwnMaster = true;
        //     this.canComplete = canComplete;
        //     this.actionCost += this.parentAction.actionCost;
        // }

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
