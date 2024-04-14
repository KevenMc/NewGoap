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
        public List<Action> chainActions = new List<Action>();
        public List<Action> branchActions = new List<Action>();
        public bool canComplete = false;

        public bool searchComplete = false;
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
            Debug.Log(
                actionName
                    + $"\n | Cost :  {actionCost}"
                    + $" \n| ActionType : {actionType.ToString()}"
                    + $"\n | Can Complete : {CanCompleteActionPlan()}"
                    + $"\nParent Acton : {parentAction?.actionName}"
            // + $"\nChild Acton : {chainActions?.actionName}"
            );
        }

        public Action()
        {
            Debug.Log("MAKING EMPTY ACTION");
        }

        public Action(Stat goal) //Use this to set grandMasterAction
        {
            Debug.Log("MAKING ACTION FROM GOAL ONLY");
            this.goal = goal;
            this.hasMasterAction = false;
            this.isOwnMaster = true;
            this.masterAction = this;
            this.grandMasterAction = this;
            this.actionName = goal.ToString();
            this.actionType = ActionType.Master_Action;
        }

        public Action(Action parentAction, Boolean isBranchAction = false)
        {
            Debug.Log("MAKING ACTION FROM PARENT ACTION");
            this.actionName = actionType.ToString();
            this.parentAction = parentAction;
            this.masterAction = parentAction.masterAction;
            if (isBranchAction)
            {
                this.parentAction.branchActions.Add(this);
            }
            else
            {
                Debug.Log(this.parentAction);
                Debug.Log(this.parentAction.chainActions);
                this.parentAction.chainActions.Add(this);
            }

            this.grandMasterAction = parentAction.grandMasterAction;
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
                    // this.parentAction.chainActions = null;
                    // this.parentAction.chainActions.Add(this);
                    break;
                case (ActionType.Equip_From_Inventory):
                case (ActionType.Collect_And_Equip):
                case (ActionType.Blueprint_Require_Item):
                    // this.actionCost += this.blueprint.actionCost;
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
                    this.actionCost += this.blueprint.actionCost;
                    this.actionName += goal.blueprint.craftedItem.itemName;
                    break;
                case (ActionType.Make_Blueprint_At_Station):
                    this.actionCost += this.blueprint.actionCost;
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
        }

        public Boolean CanCompleteActionPlan()
        {
            if (canComplete)
                return true;

            if (chainActions.Count > 0)
            {
                foreach (Action chainAction in chainActions)
                {
                    if (chainAction.CanCompleteActionPlan())
                    {
                        return true;
                    }
                }
            }

            foreach (Action branchAction in branchActions)
            {
                if (!branchAction.CanCompleteActionPlan())
                {
                    return false;
                }
            }
            if (branchActions.Count > 0)
            {
                canComplete = true;
            }
            return canComplete;
        }

        public void LogMasterAction()
        {
            Debug.Log(grandMasterAction.LogActionPlan());
        }

        public void CompleteSearch()
        {
            searchComplete = true;
            if (parentAction == null)
            {
                return;
            }
            if (parentAction.chainActions.Contains(this))
            {
                parentAction.CompleteSearch();
            }
            if (parentAction.branchActions.Count > 0)
            {
                foreach (Action branchAction in parentAction.branchActions)
                {
                    if (!branchAction.searchComplete)
                    {
                        return;
                    }
                }
                parentAction.CompleteSearch();
            }
        }

        // public bool CanCompleteGrandMasterActionPlan(){

        // }
        // public bool CanCompleteActionPlan(){

        // }


        public void RemoveAction()
        {
            if (parentAction.chainActions.Contains(this))
            {
                parentAction.chainActions.Remove(this);
                if (parentAction.chainActions.Count == 0)
                {
                    parentAction.RemoveAction();
                }
            }
        }

        public void RemoveOtherActions()
        {
            if (parentAction != null)
            {
                if (parentAction.chainActions.Contains(this))
                {
                    List<Action> newChainActions = new List<Action> { this }; // Create a new list with only this action
                    parentAction.chainActions = newChainActions; // Update the parent action's chainActions list
                    parentAction.RemoveOtherActions();
                }
                if (parentAction.branchActions.Count > 0)
                {
                    foreach (Action branchAction in parentAction.branchActions)
                    {
                        if (!branchAction.searchComplete)
                        {
                            return;
                        }
                    }
                    parentAction.RemoveOtherActions();
                }
            }
        }

        public string LogActionPlan()
        {
            // Initialize the log string
            string logString = "";
            // Log details of the current action
            logString += $"{{\"Action Name\": \"{actionName}\",";
            logString += $"\"Parent Acton\" : \"{parentAction?.actionName}\",";
            logString += $"\"Master Acton\" : \"{masterAction.actionName}\",";
            logString += $"\"Action Cost\" : \"{actionCost}\",";
            logString += $"\"Can Complete\" : \"{canComplete}\",";
            logString += $"\"Action Type\" : \"{actionType.ToString()}\",";
            logString += $"\"Search Complete\" : \"{searchComplete}\"";

            // Add sub-actions details
            if (branchActions.Count > 0)
            {
                logString += $",\"Branch Actions\":[";
                for (int i = 0; i < branchActions.Count; i++)
                {
                    logString += branchActions[i].LogActionPlan();
                    if (i < branchActions.Count - 1)
                    {
                        logString += ",";
                    }
                }
                logString += "]";
            }
            // Add child action details if present
            else if (chainActions.Count > 0)
            {
                logString += $",\"Chain Actions\":[";
                for (int i = 0; i < chainActions.Count; i++)
                {
                    logString += chainActions[i].LogActionPlan();
                    if (i < chainActions.Count - 1)
                    {
                        logString += ",";
                    }
                }
                logString += "]";
            }
            logString += "}";

            return logString;
        }
    }
}
