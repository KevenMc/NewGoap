using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        public void SetGoal(Stat goal)
        {
            actionList.Clear();
            actionList.Add(new Action(goal));
            masterAction = actionList[0];
            grandMasterAction = masterAction;
            currentAgent.requiresNewAction = true;
            Debug.Log(goal.ToString());
            // Debug.Log(currentAgent);
            currentAgent.currentGoal = goal.statType.ToString();
        }

        public void PlanAction(Agent agent)
        {
            currentLocation = agent.transform.position;
            currentAgent = agent;
            agent.statHandler.UpdateGoals();
            List<Stat> currentGoals = agent.statHandler.currentGoals;

            if (currentGoals.Count == 0)
            {
                return;
            }
            Stat topStat = currentGoals[0];

            if (topStat.IsUrgent() && agent.statHandler.currentGoal != topStat)
            {
                agent.statHandler.currentGoal = topStat;
                // Debug.Log(topStat.ToString());

                SetGoal(topStat);
                // Debug.Log(topStat.ToString());
                if (RecursiveFindAction())
                {
                    agent.SetMasterAction(masterAction);
                    return;
                }
            }
            else if (currentAgent.requiresNewAction)
            {
                foreach (Stat stat in currentGoals)
                {
                    SetGoal(stat);
                    if (RecursiveFindAction())
                    {
                        agent.SetMasterAction(masterAction);
                        return;
                    }
                }
            }
            currentAgent = null;
        }

        public Boolean RecursiveFindAction()
        {
            if (actionList.Count == 0)
            {
                Debug.Log("No possible plan of action can be found");
                // SaveMasterActionToFile("json.json");
                currentAgent.requiresNewAction = false;

                return false;
            }

            SortActionList(actionList);

            if (
                actionList[0].CanComplete()
                && CanCompleteMasterAction(actionList[0].grandMasterAction)
            )
            {
                Debug.Log("Task planning complete");
                // SaveMasterActionToFile("json.json");
                currentAgent.requiresNewAction = false;

                return true;
            }

            ExtendAction(actionList[0]);
            return RecursiveFindAction();
        }

        public bool CanCompleteAll(Action action)
        {
            if (action.subActions.Count == 0 && action.childActions.Count == 0)
            {
                return false;
            }
            return CanCompleteMasterAction2(action);
        }

        public Boolean CanCompleteMasterAction2(Action action)
        {
            foreach (Action childAction in action.childActions)
            {
                if (!CanCompleteMasterAction2(childAction))
                {
                    return false;
                }
            }
            foreach (Action subAction in action.subActions)
            {
                Debug.Log(subAction.actionName);
                if (!CanCompleteMasterAction2(subAction))
                {
                    return false;
                }
            }

            return action.canComplete;
        }

        public Boolean CanCompleteMasterAction(Action action)
        {
            if (action.subActions.Count + action.childActions.Count == 0)
            {
                return action.canComplete;
            }

            // Debug.Log("can complete action? : " + action.actionName);
            List<Boolean> returnList = new List<bool>();
            returnList.Add(true);

            SortActionList(action.subActions);
            foreach (Action subAction in action.subActions)
            {
                returnList.Add(CanCompleteMasterAction(subAction));
            }

            foreach (Action childAction in action.childActions)
            {
                returnList.Add(CanCompleteMasterAction(childAction));
            }

            return returnList.All(b => b); //return if ALL children and subActions canComplete
        }

        public void RecursiveShowActions(Action action)
        {
            if (action.parentAction != null)
            {
                // Debug.Log(
                //     action.actionName
                //         + " -> "
                //         + action.parentAction.actionName
                //         + " | => "
                //         + action.masterAction.actionName
                //         + " | SubAction : "
                //         + action.masterAction.isSubAction
                //         + " - "
                //         + action.subActions.Count
                //         + " | Cost : "
                //         + action.actionCost
                // );

                RecursiveShowActions(action.parentAction);
            }
        }

        #region

        public void ExtendAction(Action action)
        {
            Debug.Log(action.actionName);
            ExtendActionPlanFromInventory(action);
            ExtendActionPlanFromItemMemory(action);
            ExtendActionPlanFromStationMemory(action);
            ExtendActionPlanFromBlueprintRepertoire(action);

            if (action.childActions.Count == 0 && !action.CanComplete())
            {
                // RecursiveDropActions(action);
            }
            actionList.Remove(action);
        }

        public void RecursiveDropActions(Action action)
        {
            // Debug.Log("This action line is a dead end : " + action.actionName);
            if (action.parentAction == null)
            {
                return;
            }
            action.parentAction.childActions.Remove(action);
            if (action.parentAction.childActions.Count == 0)
            {
                RecursiveDropActions(action.parentAction);
            }
        }

        private void ExtendActionPlanFromInventory(Action action)
        {
            List<ItemSO> inventoryItems = currentAgent.inventoryHandler.ReturnGoalItems(
                action.goal
            );

            switch (action.goal.statType)
            {
                case StatType.Have_Item_In_Inventory:

                    foreach (ItemSO itemData in inventoryItems)
                    {
                        Stat updateGoal = new Stat(StatType.Satisfied, itemData);
                        Action updateAction = RequireItemInInventory(action, updateGoal);

                        actionList.Add(updateAction);
                        // SaveMasterActionToFile("Temp.json");
                    }

                    break;

                case StatType.Have_Item_Equipped:

                    foreach (ItemSO itemData in inventoryItems)
                    {
                        Stat updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemData);
                        Action updateAction = EquipFromInventory(action, updateGoal, true);

                        actionList.Add(updateAction);
                        // SaveMasterActionToFile("Temp.json");
                    }

                    break;

                default:

                    {
                        foreach (ItemSO itemData in inventoryItems)
                        {
                            Stat updateGoal = new Stat(StatType.Have_Item_Equipped, itemData);
                            Action updateAction = UseEquippedItem(action, updateGoal);

                            updateGoal = new Stat(StatType.Satisfied, itemData);
                            updateAction = EquipFromInventory(updateAction, updateGoal);

                            actionList.Add(updateAction);
                        }
                    }
                    break;
            }
        }

        private void ExtendActionPlanFromItemMemory(Action action)
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

        private void ExtendActionPlanFromStationMemory(Action action)
        {
            List<Station> memoryStations =
                currentAgent.knowledgeHandler.stationMemory.ReturnGoalStations(action.goal);

            foreach (Station station in memoryStations)
            {
                Stat updateGoal = new Stat(StatType.Use_Station, station);
                Action updateAction = InteractWithStation(action, updateGoal);

                updateGoal = new Stat(StatType.Satisfied, station);
                updateAction = MoveToLocation(updateAction, updateGoal);

                actionList.Add(updateAction);
            }
        }

        public void ExtendActionPlanFromBlueprintRepertoire(Action action)
        {
            Stat updateGoal = new Stat();
            Action updateAction = action;
            List<Action> updateActions = new List<Action>();

            List<Blueprint> matchingBlueprints =
                currentAgent.knowledgeHandler.blueprintRepertoire.GetBlueprintsWithGoalStatType(
                    action.goal
                );

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
                                    updateAction = UseEquippedItem(updateAction, updateGoal);
                                }

                                updateGoal = new Stat(
                                    StatType.Have_Item_In_Inventory,
                                    blueprint.craftedItem
                                );
                                updateAction = EquipFromInventory(updateAction, updateGoal);
                                break;
                        }
                        updateActions = MakeFromInventory(updateAction, blueprint);
                        actionList.AddRange(updateActions);
                        break;

                    case (_, null): //blueprint from inventory using a tool
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
                                    updateAction = UseEquippedItem(updateAction, updateGoal);
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
                        updateGoal = new Stat(StatType.Item_Is_At_Station, blueprint.craftedItem);
                        updateAction = EquipFromStation(updateAction, updateGoal);
                        updateActions = MakeFromStation(updateAction, blueprint);
                        actionList.AddRange(updateActions);
                        break;

                    case (_, _): //blueprint from station using a tool
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
                        updateGoal = new Stat(StatType.Item_Is_At_Station, blueprint.craftedItem);
                        updateAction = EquipFromStation(updateAction, updateGoal);
                        updateActions = MakeFromStationWithTool(updateAction, blueprint);
                        actionList.AddRange(updateActions);
                        break;
                }
            }
        }

        #endregion

        #region Basic Action methods
        private static Action UseEquippedItem(Action action, Stat goal)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Use_Item, goal, goal.itemData, false);
            return updateAction;
        }

        private static Action EquipFromInventory(Action action, Stat goal, Boolean hasItem = false)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Equip_From_Inventory, goal, goal.itemData, hasItem);
            return updateAction;
        }

        private static Action EquipFromStation(Action action, Stat goal)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Equip_From_Station, goal, goal.itemData);
            return updateAction;
        }

        private static Action UnEquipItem(Action updateAction, Stat goal)
        {
            updateAction = new Action(updateAction);
            updateAction.Init(ActionType.UnEquip_To_Inventory, goal, goal.itemData, false);
            return updateAction;
        }

        private static Action RequireItemInInventory(Action action, Stat goal)
        {
            Action updateAction = new Action(action);
            updateAction.Init(ActionType.Require_Item_In_Inventory, goal, goal.itemData, true);
            return updateAction;
        }

        private static Action RequireItemSubAction(Action action, Stat goal)
        {
            Action updateAction = new Action(action, true);
            updateAction.Init(ActionType.Require_Item_In_Inventory, goal, goal.itemData);
            return updateAction;
        }

        private static Action MoveToEquipItem(Action action, Item item)
        {
            Stat updateGoal = new Stat(StatType.Move_To_Item_Location, item);
            Action updateAction = CollectAndEquip(action, updateGoal);

            updateGoal = new Stat(StatType.Satisfied, item);
            updateAction = MoveToLocation(updateAction, updateGoal);
            return updateAction;
        }

        private static Action CollectAndEquip(Action updateAction, Stat goal)
        {
            updateAction = new Action(updateAction);
            updateAction.Init(ActionType.Collect_And_Equip, goal, goal.item);
            return updateAction;
        }

        private static Action MoveToLocation(
            Action updateAction,
            Stat goal,
            Boolean canMoveToLocation = true
        )
        {
            updateAction = new Action(updateAction);
            updateAction.Init(ActionType.Move_To_Location, goal, goal.location, canMoveToLocation);
            return updateAction;
        }
        #endregion

        #region Interactive Action Methods

        private List<Action> MakeFromInventory(Action action, Blueprint blueprint)
        {
            List<Action> updateActions = new List<Action>();

            Action updateAction = new Action(action); //make item one all required items are satisfied
            updateAction.Init(ActionType.Make_Blueprint_From_Inventory, blueprint);

            List<Action> subActions = new List<Action>();
            foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems) // add required items to subaction
            {
                Stat updateGoal = new Stat(
                    StatType.Have_Item_In_Inventory,
                    itemRequirement.itemData
                );
                Action subAction = RequireItemSubAction(updateAction, updateGoal);

                updateActions.Add(subAction);
            }

            return updateActions;
        }

        private List<Action> MakeFromInventoryWithTool(Action action, Blueprint blueprint)
        {
            List<Action> updateActions = new List<Action>();

            Stat updateGoal = new Stat(StatType.Have_Item_Equipped, blueprint.requiredTool);
            Action updateAction = UnEquipItem(action, updateGoal);

            updateAction = new Action(updateAction); //make item one all required items are satisfied
            updateAction.Init(ActionType.Make_Blueprint_From_Inventory, blueprint);

            updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint.requiredTool);
            updateAction = EquipFromInventory(updateAction, updateGoal);

            List<Action> subActions = new List<Action>();

            updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint.requiredTool);
            Action subAction = RequireItemSubAction(updateAction, updateGoal);

            updateActions.Add(subAction);
            foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems) // add required items to subaction
            {
                updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData);
                subAction = RequireItemSubAction(updateAction, updateGoal);

                updateActions.Add(subAction);
            }

            return updateActions;
        }

        private List<Action> MakeFromStation(Action action, Blueprint blueprint)
        {
            List<Action> updateActions = new List<Action>();

            Action updateAction = new Action(action); //make item one all required items are satisfied
            updateAction.Init(ActionType.Make_Blueprint_From_Station, blueprint);

            Stat updateGoal = new Stat(StatType.Be_At_Station, blueprint.craftingStation);
            updateAction = MoveToLocation(updateAction, updateGoal, false);

            List<Action> subActions = new List<Action>();

            foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems) // add required items to subaction
            {
                updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData);
                Action subAction = RequireItemSubAction(updateAction, updateGoal);

                updateActions.Add(subAction);
            }

            return updateActions;
        }

        private List<Action> MakeFromStationWithTool(Action action, Blueprint blueprint)
        {
            List<Action> updateActions = new List<Action>();

            Stat updateGoal = new Stat(StatType.Have_Item_Equipped, blueprint.requiredTool);
            Action updateAction = UnEquipItem(action, updateGoal);

            updateAction = new Action(updateAction); //make item one all required items are satisfied
            updateAction.Init(ActionType.Make_Blueprint_From_Station, blueprint);

            updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint.requiredTool);
            updateAction = EquipFromInventory(updateAction, updateGoal);

            updateGoal = new Stat(StatType.Be_At_Station, blueprint.craftingStation);
            updateAction = MoveToLocation(updateAction, updateGoal, false);

            List<Action> subActions = new List<Action>();

            updateGoal = new Stat(StatType.Have_Item_In_Inventory, blueprint.requiredTool);
            Action subAction = RequireItemSubAction(updateAction, updateGoal);
            updateActions.Add(subAction);

            foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems) // add required items to subaction
            {
                updateGoal = new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData);
                subAction = RequireItemSubAction(updateAction, updateGoal);

                updateActions.Add(subAction);
            }

            return updateActions;
        }

        private static Action InteractWithStation(Action action, Stat goal)
        {
            Action updateAction = action;
            updateAction = new Action(updateAction);
            updateAction.Init(ActionType.Interact_With_Station, goal, goal.station.stationData);
            return updateAction;
        }

        #endregion

        public void SortActionList(List<Action> actionList)
        {
            actionList.Sort((action1, action2) => action1.actionCost.CompareTo(action2.actionCost));
        }

        public static float GetDistance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        public static float GetDistance(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        #region

        public void SaveMasterActionToFile(string filePath)
        {
            JSONAction jSONAction = new JSONAction(masterAction);

            // Convert the dictionary to JSON
            string jsonData = JsonUtility.ToJson(jSONAction);
            jsonData = jsonData
                .Replace("\\n", "")
                .Replace("\\", "")
                .Replace("\"{", "{")
                .Replace("}\"", "}");

            // Debug.Log("json data : " + jsonData);

            // Save the JSON data to a file
            File.WriteAllText("Assets/" + filePath, jsonData);

            Debug.Log("MasterAction saved to file: " + filePath);
        }
        #endregion
    }
}
