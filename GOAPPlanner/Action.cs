using System;
using System.Collections.Generic;
using System.Globalization;
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

        public bool hasSubActions = false;

        // public Boolean isSubAction = false;
        public Boolean isLastSubAction = false;
        public ActionStatus actionStatus = ActionStatus.WaitingToExecute;
        public AnimationClip animation;
        #endregion

        #region
        public Agent delegateAgent;
        public Agent receiverAgent;
        public float actionCost = 1;
        public ItemSO itemData;
        public StationSO stationData;
        public Item item;
        public Vector3 location;
        public Blueprint blueprint;
        #endregion


        public void LogAction()
        {
            Debug.Log("o0o0o0o0o0o0o0o0o0o0o0o0o0o0o0o0o0o0o");
            Debug.Log(
                actionName + " | Cost :  " + actionCost + " | ActionType : " + actionType.ToString()
            );
            Debug.Log("---------Parent--------");
            LogParent();
        }

        private void LogParent()
        {
            Debug.Log(
                actionName
                    + " | Cost :  "
                    + actionCost
                    + " | Is Grand Master Action "
                    + isOwnMaster
                    + " | ActionType : "
                    + actionType.ToString()
            );
            if (parentAction != null)
            {
                parentAction.LogParent();
            }
        }

        public Action(Stat goal) //Use this to set grandMasterAction
        {
            this.goal = goal;
            this.hasMasterAction = false;
            this.isOwnMaster = true;
            this.masterAction = this;
            this.grandMasterAction = this;
            this.actionName = goal.ToString();
            this.actionType = ActionType.Master_Action;
        }

        public Action(Action parentAction, Boolean isSubAction = false)
        {
            this.parentAction = parentAction;
            this.actionName = actionType.ToString();

            this.grandMasterAction = parentAction.grandMasterAction;
            if (isSubAction)
            {
                this.masterAction = parentAction;
                parentAction.subActions.Add(this);
                if (actionType == ActionType.Require_Move_To_Location)
                {
                    this.isLastSubAction = true;
                }
            }
            else
            {
                this.masterAction = parentAction.masterAction;
                parentAction.childActions.Add(this);
            }
        }

        public void Init()
        {
            actionCost += parentAction.actionCost;
        }

        public void Init(
            ActionType actionType,
            Stat goal,
            Boolean canComplete = false,
            float addCost = 0
        )
        {
            this.actionCost += this.parentAction.actionCost;
            this.goal = goal;
            this.actionType = actionType;
            this.itemData = goal.itemData;
            this.item = goal.item;
            this.stationData = goal.stationData;
            this.location = goal.location;
            this.blueprint = goal.blueprint;
            this.delegateAgent = goal.agent;
            this.actionName = actionType.ToString() + " : ";

            switch (actionType)
            {
                case (ActionType.Use_Item):
                    this.actionCost += goal.itemData.itemUseCost;
                    this.actionName += goal.itemData.itemName;
                    break;
                case (ActionType.UnEquip_To_Inventory):
                case (ActionType.Require_Item_In_Inventory):
                case (ActionType.Equip_From_Inventory):
                case (ActionType.Collect_And_Equip):
                case (ActionType.Blueprint_Require_Item):
                    this.actionName += goal.itemData.itemName;
                    break;
                case (ActionType.Interact_With_Station):
                    this.actionName += goal.stationData.stationName;
                    break;
                case (ActionType.Move_To_Location):
                    this.actionName += goal.location;
                    this.actionCost += addCost;
                    break;
                case (ActionType.Require_Move_To_Location):
                    this.actionName += goal.stationData.stationName;
                    break;
                case (ActionType.Make_Blueprint_From_Inventory):
                    Debug.Log("vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv");
                    Debug.Log(this.parentAction.actionName);
                    Debug.Log(this.parentAction.parentAction.actionName);

                    this.actionName += goal.blueprint.craftedItem.itemName;
                    break;
                case (ActionType.Make_Blueprint_At_Station):
                    // this.actionCost += goal.blueprint.actionCost;
                    this.actionName +=
                        goal.blueprint.craftedItem.itemName
                        + " at "
                        + goal.blueprint.craftingStation.stationName;
                    break;
                case (ActionType.Equip_From_Station):
                    this.actionName +=
                        goal.itemData.itemName + " from " + goal.stationData.stationName;
                    break;
                case (ActionType.Delegate_Action):
                    this.delegateAgent = goal.agent;
                    this.actionName += goal.agent.agentName + "\n =>> " + goal.delegateGoal;
                    break;
                case (ActionType.Move_To_Agent):
                case (ActionType.Return_Delegate_Action):
                case (ActionType.Receive_Delegate_Action):
                    this.delegateAgent = goal.agent;
                    this.actionName += goal.agent.agentName;
                    break;
                default:
                    break;
            }

            this.canComplete = canComplete;
            if (actionType == ActionType.Require_Move_To_Location)
            {
                this.isLastSubAction = true;
            }
        }

        public Boolean CanComplete()
        {
            foreach (Action subAction in subActions)
            {
                if (!subAction.CanComplete())
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
