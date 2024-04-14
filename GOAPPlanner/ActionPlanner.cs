using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GOAP
{
    public class ActionPlanner : MonoBehaviour
    {
        private Agent currentAgent;
        public List<Action> actionList = new List<Action>();
        public Action masterAction;
        public Action grandMasterAction;
        public Vector3 currentLocation;
        public Boolean canDelegate = true;
        public List<List<Action>> duplicateActions = new List<List<Action>>();

        public void SetGoal(Stat goal)
        {
            if (goal.agent != null)
            {
                // canDelegate = false;
            }
            actionList.Clear();
            actionList.Add(new Action(goal));
            // SortActionList(actionList);

            masterAction = actionList[0];
            grandMasterAction = masterAction;
            currentAgent.requiresNewAction = true;
            currentAgent.currentGoal = goal.statType.ToString();
        }

        public void PlanAction(Agent agent)
        {
            Debug.Log("Plan Action");
            canDelegate = true;
            currentLocation = agent.transform.position;
            currentAgent = agent;
            agent.statHandler.UpdateGoals();
            List<Stat> currentGoals = agent.statHandler.currentGoals;

            if (currentGoals.Count == 0)
            {
                grandMasterAction = null;
                agent.actionHandler.canPerformAction = false;
                return;
            }

            Action lowestCostPlan = null;
            float lowestCost = float.MaxValue;

            foreach (Stat stat in currentGoals)
            {
                SetGoal(stat);
                Debug.Log("Stat goal  setting  : " + stat);
                if (IterateThroughActions())
                {
                    // Get the lowest-cost plan
                    if (masterAction.actionCost < lowestCost)
                    {
                        lowestCost = masterAction.actionCost;
                        lowestCostPlan = masterAction.grandMasterAction;
                    }
                }
            }

            // Set the master action to the lowest-cost plan
            if (lowestCostPlan != null)
            {
                agent.SetMasterAction(lowestCostPlan);
                RemoveUncompletableActionsRecursive(lowestCostPlan);
                agent.actionHandler.canPerformAction = true;
            }

            currentAgent = null;
        }

        int iterateNumber = 0;

        public Boolean IterateThroughActions()
        {
            Debug.Log("Iterate Through Actions");
            iterateNumber++;
            if (iterateNumber > 99)
            {
                Debug.Log(
                    "CRASH AND BURN AND CRASH AND BURN AND CRASH AND BURN AND CRASH AND BURN AND CRASH AND BURN AND CRASH AND BURN AND "
                );
                return false;
            }
            // Make a copy of the action list to iterate over
            List<Action> actionListCopy = new List<Action>(actionList);
            Action action;
            // Sort the copied action list by cost
            SortActionList(actionListCopy);
            if (actionListCopy.Count > 0)
            {
                action = actionListCopy[0];

                actionListCopy.RemoveAt(0);

                if (action.canComplete)
                {
                    action.CompleteSearch();
                    action.RemoveOtherActions();
                }
                if (action.grandMasterAction.searchComplete)
                {
                    masterAction = action.grandMasterAction;
                    Debug.Log("-----CAN COMPLETE ACTION-----");
                    Debug.Log(masterAction.LogActionPlan());
                    Debug.Log("-----------------------------");

                    // SaveMasterActionToFile("json.json", action.grandMasterAction);
                    // currentAgent.requiresNewAction = false;
                    currentAgent.actionHandler.canPerformAction = true;
                    return true;
                }
                else
                {
                    Debug.Log(action.grandMasterAction.LogActionPlan());

                    ExtendAction(action);
                }
            }
            Debug.Log(masterAction.LogActionPlan());

            if (actionList.Count > 0)
            {
                return IterateThroughActions();
            }
            // If no action can be completed, return false
            Debug.Log(
                "NO ACTION CAN BE FOUIND::NO ACTION CAN BE FOUIND::NO ACTION CAN BE FOUIND::NO ACTION CAN BE FOUIND::"
            );
            return false;
        }

        #region Extend Actions
        public void ExtendAction(Action action)
        {
            actionList.Remove(action);

            switch (action.goal.statType)
            {
                // case StatType.Return_Delegate_Action:
                //     actionList.Add(ReturnDelegateAction(action));
                //     break;

                // case StatType.Storage:
                //     actionList.Add(StoreItem(action));
                //     break;

                default:
                    PlanFromInventory(action);
                    PlanFromKnownItems(action);
                    PlanFromStationMemory(action);
                    PlanFromBlueprintRepertoire(action);
                    if (canDelegate)
                        PlanFromDelegate(action);
                    break;
            }

            // else
            // {
            //     action.RemoveAction();
            // }
        }

        public void RecursiveDropActions(Action action)
        {
            if (action.parentAction == null)
            {
                return;
            }
            action.parentAction.chainActions.RemoveAt(0);
        }

        private void PlanFromInventory(Action action)
        {
            List<ItemSO> inventoryItems = currentAgent.inventoryHandler.ReturnGoalItems(
                action.goal,
                currentAgent.inventory
            );

            switch (action.goal.statType)
            {
                case StatType.Have_Item_In_Inventory:
                    foreach (ItemSO itemData in inventoryItems)
                    {
                        Stat updateGoal = new Stat(StatType.Satisfied, itemData);
                        Action updateAction = RequireItemInInventory(action, updateGoal);
                        actionList.Add(updateAction);
                    }
                    break;

                case StatType.Have_Item_Equipped:
                    foreach (ItemSO itemData in inventoryItems)
                    {
                        Stat updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemData);
                        Action updateAction = EquipFromInventory(action, updateGoal, true);
                        actionList.Add(updateAction);
                    }
                    break;

                default:
                    foreach (ItemSO itemData in inventoryItems)
                    {
                        Stat updateGoal = new Stat(StatType.Have_Item_Equipped, itemData);
                        Action updateAction = UseEquippedItem(action, updateGoal);
                        updateGoal = new Stat(StatType.Satisfied, itemData);
                        updateAction = EquipFromInventory(updateAction, updateGoal, true);
                        actionList.Add(updateAction);
                    }
                    break;
            }
        }

        private void PlanFromKnownItems(Action action)
        {
            Action updateAction = action;
            List<Item> memoryItems = new List<Item>();
            switch (action.goal.statType)
            {
                case StatType.Have_Item_In_Inventory:
                case StatType.Have_Item_Equipped:
                    memoryItems.AddRange(
                        currentAgent.knowledgeHandler.itemMemory.ItemsByItem(action.goal)
                    );
                    foreach (Item item in memoryItems)
                    {
                        Stat updateGoal = new Stat(StatType.Have_Item_Equipped, item.itemData);
                        if (updateAction.goal.statType == StatType.Have_Item_In_Inventory)
                        {
                            updateAction = UnEquipItem(updateAction, updateGoal);
                        }
                        updateAction = MoveToEquipItem(updateAction, item);
                        updateAction.Init();
                        actionList.Add(updateAction);
                    }
                    break;

                default: //Move to item -> equip item -> use item
                    memoryItems.AddRange(
                        currentAgent.knowledgeHandler.itemMemory.ItemsByStat(action.goal)
                    );
                    foreach (Item item in memoryItems)
                    {
                        Boolean isAtLocation =
                            currentAgent.distanceToArrive
                            >= GetDistance(currentLocation, item.location);

                        Stat updateGoal = new Stat(StatType.Have_Item_Equipped, item.itemData);
                        updateAction = UseEquippedItem(action, updateGoal);
                        updateAction = MoveToEquipItem(updateAction, item);
                        actionList.Add(updateAction);
                    }
                    break;
            }
        }

        private void PlanFromStationMemory(Action action)
        {
            List<Station> memoryStations =
                currentAgent.knowledgeHandler.stationMemory.ReturnGoalStations(action.goal);
            if (action.actionType == ActionType.Require_Move_To_Location)
            {
                foreach (Station station in memoryStations)
                {
                    Stat updateGoal = new Stat(StatType.Satisfied, station);
                    Action updateAction = MoveToLocation(action, updateGoal);
                    actionList.Add(updateAction);
                }
            }
            else
            {
                foreach (Station station in memoryStations)
                {
                    Stat updateGoal = new Stat(StatType.Use_Station, station);
                    Action updateAction = InteractWithStation(action, updateGoal);
                    updateGoal = new Stat(StatType.Satisfied, station);
                    updateAction = MoveToLocation(updateAction, updateGoal);
                    actionList.Add(updateAction);
                }
            }
        }

        public void PlanFromBlueprintRepertoire(Action action)
        {
            Stat updateGoal = new Stat();
            Action updateAction = action;
            List<Action> updateActions = new List<Action>();

            List<Blueprint> matchingBlueprints =
                currentAgent.knowledgeHandler.blueprintRepertoire.GetBlueprintsWithGoalStatType(
                    action.goal
                );

            if (matchingBlueprints.Count > 0)
            {
                Debug.Log("Checking Blueprints");
            }
            foreach (Blueprint blueprint in matchingBlueprints)
            {
                switch ((blueprint.requiredTool, blueprint.craftingStation))
                {
                    case (null, null): //make blueprint from inventory with no tools
                        switch (action.goal.statType)
                        {
                            case StatType.Have_Item_In_Inventory:
                                break;

                            default:
                                if (action.goal.statType != StatType.Have_Item_Equipped)
                                {
                                    updateGoal = new Stat(
                                        StatType.Have_Item_Equipped,
                                        blueprint.craftedItem
                                    );
                                    Debug.Log(blueprint.craftedItem);
                                    Debug.Log(updateAction.actionName);
                                    Debug.Log(action.actionName);

                                    updateAction = UseEquippedItem(action, updateGoal);
                                }
                                updateGoal = new Stat(
                                    StatType.Have_Item_In_Inventory,
                                    blueprint.craftedItem
                                );
                                // updateAction = EquipFromInventory(updateAction, updateGoal);
                                break;
                        }
                        updateActions = MakeFromInventory(updateAction, blueprint);
                        actionList.AddRange(updateActions);

                        break;

                    case (_, null): //blueprint from inventory using a tool
                        Debug.Log(
                            "Case _ null---- Case _ nullCase _ null---- Case _ null---- Case _ null---- Case _ null---- Case _ null---- "
                        );
                        switch (action.goal.statType)
                        {
                            case StatType.Have_Item_In_Inventory:
                                break;

                            default:
                                if (action.goal.statType != StatType.Have_Item_Equipped)
                                {
                                    Debug.Log(
                                        "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"
                                    );
                                    updateGoal = new Stat(
                                        StatType.Have_Item_Equipped,
                                        blueprint.craftedItem
                                    );
                                    updateAction = UseEquippedItem(action, updateGoal);
                                }
                                updateGoal = new Stat(
                                    StatType.Have_Item_In_Inventory,
                                    blueprint.craftedItem
                                );
                                updateAction = EquipFromInventory(updateAction, updateGoal);

                                break;
                        }

                        updateActions = MakeFromInventoryWithTool(updateAction, blueprint);
                        actionList.AddRange(updateActions);
                        break;

                    case (null, _): //blueprint from station with no tools
                        Debug.Log(
                            "NULL AND _ : NULL AND _ : NULL AND _ : NULL AND _ : NULL AND _ : NULL AND _ : NULL AND _ : "
                        );
                        switch (action.goal.statType)
                        {
                            case StatType.Have_Item_In_Inventory:
                                updateGoal = new Stat(
                                    StatType.Have_Item_Equipped,
                                    blueprint.craftedItem
                                );
                                updateAction = UnEquipItem(updateAction, updateGoal);
                                break;

                            case StatType.Have_Item_Equipped:
                                break;

                            default:
                                updateGoal = new Stat(
                                    StatType.Item_Is_At_Station,
                                    blueprint.craftedItem
                                );
                                updateAction = UseEquippedItem(updateAction, updateGoal);
                                break;
                        }
                        updateGoal = new Stat(StatType.Item_Is_At_Station, blueprint);
                        // updateAction = EquipFromStation(updateAction, updateGoal);
                        updateActions = MakeFromStation(updateAction, blueprint);
                        actionList.AddRange(updateActions);
                        break;

                    case (_, _): //blueprint from station using a tool
                        Debug.Log("Case _ _");
                        switch (action.goal.statType)
                        {
                            case StatType.Have_Item_In_Inventory:
                                updateGoal = new Stat(
                                    StatType.Have_Item_Equipped,
                                    blueprint.craftedItem
                                );
                                updateAction = UnEquipItem(updateAction, updateGoal);
                                break;

                            case StatType.Have_Item_Equipped:
                                break;

                            default:
                                updateGoal = new Stat(
                                    StatType.Item_Is_At_Station,
                                    blueprint.craftedItem
                                );
                                updateAction = UseEquippedItem(updateAction, updateGoal);

                                break;
                        }
                        updateGoal = new Stat(StatType.Item_Is_At_Station, blueprint);
                        updateAction = EquipFromStation(updateAction, updateGoal);
                        updateActions = MakeFromStationWithTool(updateAction, blueprint);
                        actionList.AddRange(updateActions);
                        break;
                }
            }
        }

        public void PlanFromKnownInventories(Action action)
        {
            Action updateAction = action;

            switch (action.goal.statType)
            {
                case StatType.Have_Item_In_Inventory:
                case StatType.Have_Item_Equipped:
                    foreach (Inventory inventory in currentAgent.knowledgeHandler.inventories)
                    {
                        List<ItemSO> items = currentAgent.inventoryHandler.ReturnGoalItems(
                            action.goal,
                            inventory
                        );
                        foreach (ItemSO item in items)
                        {
                            if (updateAction.goal.statType == StatType.Have_Item_In_Inventory)
                            {
                                Stat invGoal = new Stat(StatType.Have_Item_In_Inventory, items[0]);
                                updateAction = UnEquipItem(updateAction, invGoal);
                            }
                            Stat updateGoal = new Stat(StatType.Have_Item_Equipped, items[0]);
                            updateAction = EquipFromStorage(updateAction, updateGoal);
                            actionList.Add(updateAction);
                        }
                    }
                    break;

                default:
                    foreach (Inventory inventory in currentAgent.knowledgeHandler.inventories)
                    {
                        List<ItemSO> items = currentAgent.inventoryHandler.ReturnGoalItems(
                            action.goal,
                            inventory
                        );

                        foreach (ItemSO item in items)
                        {
                            Stat invGoal = new Stat(updateAction.goal.statType, items[0]);
                            updateAction = UseEquippedItem(updateAction, invGoal);

                            Stat updateGoal = new Stat(StatType.Have_Item_Equipped, items[0]);
                            updateAction = EquipFromStorage(updateAction, updateGoal);
                            actionList.Add(updateAction);
                        }
                    }
                    break;
            }
        }

        public void PlanFromDelegate(Action action)
        {
            if (action.actionType != ActionType.Require_Item_In_Inventory)
                return;

            List<Agent> agents = currentAgent.relationshipHandler.ReturnDelegateAgents(action.goal);
            foreach (Agent agent in agents)
            {
                Stat updateGoal = new Stat(StatType.Delegate_Action, agent, action.goal);
                Action updateAction = DelegateAction(action, updateGoal);
                actionList.Add(updateAction);
            }
        }

        public Action ReturnDelegateAction(Action action)
        {
            Stat updateGoal = new Stat(StatType.Have_Item_In_Inventory, action.goal.itemData);
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Require_Item_In_Inventory, updateGoal);
            return updateAction;
        }
        #endregion

        #region Basic Action methods
        private Action UseEquippedItem(Action action, Stat goal)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Use_Item, goal);
            Debug.Log("---------------------------");
            Debug.Log(updateAction.actionName);
            Debug.Log(updateAction.parentAction.actionName);
            Debug.Log("---------------------------");

            return updateAction;
        }

        private Action EquipFromInventory(Action action, Stat goal, Boolean hasItem = false)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Equip_From_Inventory, goal, hasItem);
            return updateAction;
        }

        private Action EquipFromStation(Action action, Stat goal)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Equip_From_Station, goal);
            return updateAction;
        }

        private Action UnEquipItem(Action updateAction, Stat goal)
        {
            updateAction = new Action(updateAction);
            updateAction.Init(ActionType.UnEquip_To_Inventory, goal);
            return updateAction;
        }

        private Action RequireItemInInventory(Action action, Stat goal)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Require_Item_In_Inventory, goal);
            return updateAction;
        }

        private Action RequireItemBranchAction(Action action, Stat goal)
        {
            Action updateAction = new Action(action, true);
            updateAction.Init(ActionType.Require_Item_In_Inventory, goal);
            return updateAction;
        }

        private Action MoveToEquipItem(Action action, Item item)
        {
            Stat updateGoal = new Stat(StatType.Move_To_Location, item);
            Action updateAction = CollectAndEquip(action, updateGoal);

            updateGoal = new Stat(StatType.Satisfied, item);
            updateAction = MoveToLocation(updateAction, updateGoal);
            return updateAction;
        }

        private Action CollectAndEquip(Action updateAction, Stat goal)
        {
            updateAction = new Action(updateAction);
            updateAction.Init(ActionType.Collect_And_Equip, goal);
            return updateAction;
        }

        private Action MoveToLocation(
            Action updateAction,
            Stat goal,
            Boolean canMoveToLocation = true
        )
        {
            updateAction = new Action(updateAction);
            float distance = Vector3.Distance(currentAgent.transform.position, goal.location);
            updateAction.Init(ActionType.Move_To_Location, goal, canMoveToLocation, distance);

            return updateAction;
        }

        public Action RequireMoveToLocation(Action updateAction, Stat goal, bool canMove = true)
        {
            updateAction = new Action(updateAction, canMove);
            updateAction.Init(ActionType.Require_Move_To_Location, goal);

            return updateAction;
        }
        #endregion

        #region Blueprint Action Methods
        private List<Action> MakeFromInventory(Action action, Blueprint blueprint)
        {
            List<Action> updateActions = new List<Action>();
            List<Action> branchActions = new List<Action>();

            Action updateAction = new Action(action); //make item one all required items are satisfied
            Stat updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint);
            updateAction.Init(ActionType.Make_Blueprint_From_Inventory, updateGoal);

            foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems) // add required items to branchAction
            {
                updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData);
                Action branchAction = RequireItemBranchAction(updateAction, updateGoal);

                updateActions.Add(branchAction);
            }

            return updateActions;
        }

        private List<Action> MakeFromInventoryWithTool(Action action, Blueprint blueprint)
        {
            List<Action> updateActions = new List<Action>();
            List<Action> branchActions = new List<Action>();

            Action updateAction = new Action(action); //make item one all required items are satisfied
            Stat updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint);
            updateAction.Init(ActionType.Make_Blueprint_From_Inventory, updateGoal);

            //line
            // Debug.Log("TOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOL");
            // Debug.Log(blueprint.requiredTool);

            updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint.requiredTool);
            updateActions.Add(RequireItemBranchAction(updateAction, updateGoal));

            // updateActions.Add(branchAction);
            // foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems) // add required items to branchAction
            // {
            //     updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData);
            //     updateActions.Add(RequireItemBranchAction(updateAction, updateGoal));
            // }
            //line

            foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems) // add required items to branchAction
            {
                updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData);
                updateActions.Add(RequireItemBranchAction(updateAction, updateGoal));
            }

            return updateActions;
            // List<Action> updateActions = new List<Action>();
            // List<Action> branchActions = new List<Action>();

            // // Stat updateGoal = new Stat(StatType.Have_Item_Equipped, blueprint.requiredTool);
            // // Action updateAction = UnEquipItem(action, updateGoal);

            // Stat updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint);
            // Action updateAction = new Action(action); //make item one all required items are satisfied
            // updateAction.Init(ActionType.Make_Blueprint_From_Inventory, updateGoal);

            // updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint.requiredTool);
            // updateAction = EquipFromInventory(updateAction, updateGoal);

            // Debug.Log("TOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOLTOOL");
            // Debug.Log(blueprint.requiredTool);
            // updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint.requiredTool);
            // Action branchAction = RequireItemBranchAction(updateAction, updateGoal);

            // updateActions.Add(branchAction);
            // foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems) // add required items to branchAction
            // {
            //     updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData);
            //     branchAction = RequireItemBranchAction(updateAction, updateGoal);
            //     updateActions.Add(branchAction);
            // }

            // return updateActions;
        }

        private List<Action> MakeFromStation(Action action, Blueprint blueprint)
        {
            List<Action> updateActions = new List<Action>();

            Stat updateGoal = new Stat(StatType.Blueprint, blueprint);
            Action updateAction = new Action(action); //make item one all required items are satisfied
            updateAction.Init(ActionType.Make_Blueprint_At_Station, updateGoal, false);
            updateGoal = new Stat(StatType.Be_At_Station, blueprint);

            updateAction = RequireMoveToLocation(updateAction, updateGoal, false);
            updateActions.Add(updateAction); //create goal to be at station -> this needs to be performed last
            //add in a way to select which station to move to

            List<Action> branchActions = new List<Action>();

            foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems) // add required items to branchAction
            {
                updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData);
                Action branchAction = RequireItemBranchAction(updateAction, updateGoal);
                updateActions.Add(branchAction);
            }
            return updateActions;
        }

        private List<Action> MakeFromStationWithTool(Action action, Blueprint blueprint)
        {
            List<Action> updateActions = new List<Action>();

            Stat updateGoal = new Stat(StatType.Have_Item_Equipped, blueprint.requiredTool);
            Action updateAction = UnEquipItem(action, updateGoal);

            updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint);
            updateAction = new Action(updateAction); //make item once all required items are satisfied
            updateAction.Init(ActionType.Make_Blueprint_At_Station, updateGoal);

            updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint.requiredTool);
            updateAction = EquipFromInventory(updateAction, updateGoal);

            updateGoal = new Stat(StatType.Be_At_Station, blueprint);
            updateActions.Add(RequireMoveToLocation(updateAction, updateGoal));
            //add in a way to select which station to move to

            List<Action> branchActions = new List<Action>();

            updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint.requiredTool);
            Action branchAction = RequireItemBranchAction(updateAction, updateGoal);
            updateActions.Add(branchAction);

            foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems) // add required items to branchAction
            {
                updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData);
                branchAction = RequireItemBranchAction(updateAction, updateGoal);
                updateActions.Add(branchAction);
            }

            return updateActions;
        }

        private Action InteractWithStation(Action action, Stat goal)
        {
            Action updateAction = action;
            updateAction = new Action(updateAction);
            updateAction.Init(ActionType.Interact_With_Station, goal);
            return updateAction;
        }
        #endregion

        #region World Action Method
        private Action MoveToAgent(Action updateAction, Stat goal, Boolean canMoveToLocation = true)
        {
            updateAction = new Action(updateAction);
            updateAction.Init(ActionType.Move_To_Agent, goal, canMoveToLocation);
            return updateAction;
        }

        public Action DelegateAction(Action action, Stat goal)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Receive_Delegate_Action, goal);

            updateAction = new Action(updateAction);
            updateAction.Init(ActionType.Delegate_Action, goal);

            Stat updateGoal = new Stat(StatType.Interrupt_Agent, goal.agent, goal);
            updateAction = MoveToAgent(updateAction, updateGoal);
            return updateAction;
        }

        private Action EquipFromStorage(Action action, Stat goal)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Equip_From_Storage, goal);
            Stat updateGoal = new Stat(StatType.Be_At_Station, goal.stationData);

            updateAction = MoveToLocation(updateAction, updateGoal);
            return updateAction;
        }

        private Action StoreItem(Action action)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.UnEquip_To_Storage, action.goal);

            Stat updateGoal = new Stat(StatType.Have_Item_Equipped, action.goal.itemData);
            updateAction = EquipFromInventory(updateAction, updateGoal);

            updateGoal = new Stat(StatType.Be_At_Station, action.goal.stationData);
            updateAction = MoveToLocation(updateAction, updateGoal, false);

            updateGoal = new Stat(StatType.Have_Item_In_Inventory, action.goal.itemData);
            updateAction = RequireItemInInventory(updateAction, updateGoal);
            return updateAction;
        }

        public void CollectFromStorage(Action action) { }
        #endregion

        public void SortActionList(List<Action> actionList)
        {
            actionList.Sort((action1, action2) => action1.actionCost.CompareTo(action2.actionCost));
        }

        public float GetDistance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        public float GetDistance(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        #region

        public void LogAll(Action action)
        {
            if (action.parentAction != null)
            {
                LogAll(action.parentAction);
            }
        }

        private bool RemoveUncompletableActionsRecursive(Action action)
        {
            // Base case: If the action has no child actions, it's a terminal node
            if (action.chainActions.Count == 0)
            {
                // Check if the terminal node can be completed
                if (!action.CanCompleteActionPlan())
                {
                    // If the terminal node cannot be completed, remove it from its parent's child actions
                    if (action.parentAction != null)
                    {
                        action.parentAction.chainActions.Remove(action);
                        return false; // Indicate that this branch cannot be completed
                    }
                    else
                    {
                        // If the terminal node has no parent (i.e., it's the root), just return false
                        return false;
                    }
                }
                else
                {
                    // If the terminal node can be completed, set its parent's canComplete flag to true
                    if (action.parentAction != null)
                    {
                        action.parentAction.canComplete = true;
                    }
                    return true; // Indicate that this branch can be completed
                }
            }
            else
            {
                // If the action has child actions, recursively process each child
                bool branchCompletable = true;

                if (!RemoveUncompletableActionsRecursive(action.chainActions[0]))
                {
                    // If the child branch cannot be completed, remove it from the parent's child actions
                    action.chainActions.RemoveAt(0);
                }
                else
                {
                    // If the child branch can be completed, set the flag to indicate this branch can be completed
                    branchCompletable = true;
                }

                // If any child branch can be completed, set the parent's canComplete flag to true
                if (branchCompletable && action.parentAction != null)
                {
                    action.parentAction.canComplete = true;
                }
                return branchCompletable;
            }
        }

        public void SaveMasterActionToFile(string filePath, Action action)
        {
            // JSONAction jSONAction = new JSONAction(action);

            // Convert the dictionary to JSON
            // string jsonData = JsonUtility.ToJson(jSONAction);
            // jsonData = jsonData
            //     .Replace("\\n", "")
            //     .Replace("\\", "")
            //     .Replace("\"{", "{")
            //     .Replace("}\"", "}");

            // // Save the JSON data to a file
            // File.WriteAllText("Assets/GOAP/" + filePath, jsonData);
        }
        #endregion
    }
}
