using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP
{
    public class ActionPlanner : MonoBehaviour
    {
        private Agent currentAgent;
        public List<Action> actionList = new List<Action>();
        public Action masterAction;
        public Vector3 currentLocation;

        public void SetGoal(Stat goal)
        {
            actionList.Clear();
            actionList.Add(new Action(goal));
            masterAction = actionList[0];
            currentAgent.requiresNewAction = true;
            Debug.Log(goal.ToString());
            Debug.Log(currentAgent);
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
                Debug.Log(topStat.ToString());

                SetGoal(topStat);
                Debug.Log(topStat.ToString());
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
                return false;
            }

            SortActionList(actionList);

            if (actionList[0].CanComplete() && CanCompleteMasterAction(masterAction))
            {
                Debug.Log(
                    actionList[0].actionName
                        + " : "
                        + actionList[0].masterAction.actionName
                        + "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
                        + CanCompleteMasterAction(masterAction)
                );
                RecursiveShowActions(actionList[0]);
                SaveMasterActionToFile("json.json");
                currentAgent.requiresNewAction = false;

                return true;
            }

            ExtendAction(actionList[0]);
            return RecursiveFindAction();
        }

        public Boolean CanCompleteMasterAction(Action action)
        {
            Debug.Log("can complete action? : " + action.actionName);
            List<Boolean> returnList = new List<bool>();
            returnList.Add(true);
            if (action.subActions.Count > 0)
            {
                SortActionList(action.subActions);
                foreach (Action subAction in action.subActions)
                {
                    Boolean can = CanCompleteMasterAction(subAction);
                    Debug.Log(
                        "£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££"
                    );
                    Debug.Log(subAction.actionName + " : " + can);
                    Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                    returnList.Add(can);
                }
            }
            else if (action.childActions.Count > 0)
            {
                returnList.Add(CanCompleteMasterAction(action.childActions.Last()));
            }
            return returnList.All(b => b); //return if ALL children and subActions canComplete
        }

        public void RecursiveShowActions(Action action)
        {
            if (action.parentAction != null)
            {
                Debug.Log(
                    action.actionName
                        + " -> "
                        + action.parentAction.actionName
                        + " | => "
                        + action.masterAction.actionName
                        + " | SubAction : "
                        + action.masterAction.isSubAction
                        + " - "
                        + action.subActions.Count
                        + " | Cost : "
                        + action.actionCost
                );

                RecursiveShowActions(action.parentAction);
            }
        }

        #region

        public void ExtendAction(Action action)
        {
            ExtendActionPlanFromInventory(action);
            ExtendActionPlanFromItemMemory(action);
            // ExtendActionPlanFromStationMemory(action);
            ExtendActionPlanFromBlueprintRepertoire(action);

            if (action.childActions.Count == 0 && !action.CanComplete())
            {
                // RecursiveDropActions(action);
            }
            actionList.Remove(action);
        }

        public void RecursiveDropActions(Action action)
        {
            Debug.Log("This action line is a dead end : " + action.actionName);
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
                        Action newInventoryAction = new Action(action);
                        newInventoryAction.Init(
                            ActionType.Require_Item_In_Inventory,
                            action.goal,
                            itemData,
                            true
                        );

                        actionList.Add(newInventoryAction);
                        SaveMasterActionToFile("Temp.json");
                    }

                    break;

                case StatType.Have_Item_Equipped:

                    foreach (ItemSO itemData in inventoryItems)
                    {
                        Action newInventoryAction = new Action(action);
                        newInventoryAction.Init(
                            ActionType.Equip_From_Inventory,
                            action.goal,
                            itemData,
                            true
                        );

                        actionList.Add(newInventoryAction);
                        SaveMasterActionToFile("Temp.json");
                    }

                    break;

                default:

                    {
                        foreach (ItemSO itemData in inventoryItems)
                        {
                            Action newInventoryAction = new Action(action);
                            newInventoryAction.Init(
                                ActionType.Use_Item,
                                action.goal,
                                itemData,
                                false
                            );

                            Action newEquipAction = new Action(newInventoryAction);
                            newEquipAction.Init(
                                ActionType.Equip_From_Inventory,
                                newInventoryAction.goal,
                                itemData,
                                true
                            );

                            actionList.Add(newEquipAction);
                        }
                    }
                    break;
            }
        }

        private void ExtendActionPlanFromItemMemory(Action action)
        {
            Action updateAction = action;
            Debug.Log(action.actionName);

            switch (action.goal.statType)
            {
                case StatType.Have_Item_In_Inventory:
                case StatType.Have_Item_Equipped:

                    {
                        List<Item> memoryItems =
                            currentAgent.knowledgeHandler.itemMemory.ReturnGoalItemsByItem(
                                action.goal
                            );

                        foreach (Item item in memoryItems)
                        {
                            Boolean isAtLocation =
                                currentAgent.distanceToArrive
                                >= GetDistance(currentLocation, item.location);

                            if (updateAction.goal.statType == StatType.Have_Item_In_Inventory)
                            {
                                updateAction = new Action(updateAction);
                                updateAction.Init(
                                    ActionType.UnEquip_To_Inventory,
                                    new Stat(StatType.Have_Item_Equipped, item.itemData),
                                    item,
                                    false
                                );
                            }
                            updateAction = new Action(updateAction);
                            updateAction.Init(
                                ActionType.Collect_And_Equip,
                                new Stat(StatType.Move_To_Location, item.itemData),
                                item,
                                isAtLocation
                            );

                            if (!isAtLocation)
                            {
                                updateAction = new Action(updateAction);
                                updateAction.Init(
                                    ActionType.Move_To_Location,
                                    new Stat(StatType.Move_To_Location, item.itemData),
                                    item.location,
                                    true
                                );
                            }
                            actionList.Add(updateAction);
                        }
                    }
                    break;

                default: //Move to item -> equip item -> use item

                    {
                        List<Item> memoryItems =
                            currentAgent.knowledgeHandler.itemMemory.ReturnGoalItemsByStat(
                                action.goal
                            );

                        foreach (Item item in memoryItems)
                        {
                            Boolean isAtLocation =
                                currentAgent.distanceToArrive
                                >= GetDistance(currentLocation, item.location);

                            updateAction = new Action(updateAction);
                            updateAction.Init(
                                ActionType.Use_Item,
                                action.goal,
                                item.itemData,
                                false
                            );

                            updateAction = new Action(updateAction);
                            updateAction.Init(
                                ActionType.Collect_And_Equip,
                                new Stat(StatType.Move_To_Location, item.itemData),
                                item,
                                isAtLocation
                            );

                            if (!isAtLocation)
                            {
                                updateAction = new Action(updateAction);
                                updateAction.Init(
                                    ActionType.Move_To_Location,
                                    new Stat(StatType.Move_To_Location, item.itemData),
                                    item.location,
                                    true
                                );
                            }
                            actionList.Add(updateAction);
                        }
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
                Debug.Log(
                    "memoryStationsmemoryStationsmemoryStationsmemoryStationsmemoryStationsmemoryStationsmemoryStationsmemoryStationsmemoryStationsmemoryStationsmemoryStations"
                );
                Boolean isAtLocation =
                    currentAgent.distanceToArrive >= GetDistance(currentLocation, station.location);

                Action updateAction = action;
                updateAction = new Action(updateAction);
                updateAction.Init(
                    ActionType.Interact_With_Station,
                    new Stat(StatType.Use_Station, station.stationData),
                    station.stationData
                );

                updateAction = new Action(updateAction);
                updateAction.Init(
                    ActionType.Move_To_Location,
                    new Stat(StatType.Move_To_Location, station.stationData),
                    station.location,
                    true
                );

                actionList.Add(updateAction);
            }
        }

        public void ExtendActionPlanFromBlueprintRepertoire(Action action)
        {
            Action updateAction = action;

            List<Blueprint> matchingBlueprints = new List<Blueprint>();

            switch (action.goal.statType)
            {
                case StatType.Have_Item_In_Inventory:
                    Debug.Log(
                        "11111111111111111111111111111111111111111111111111111111111111111111111111111111"
                    );
                    matchingBlueprints =
                        currentAgent.knowledgeHandler.blueprintRepertoire.GetBlueprintsWithGoalStatType(
                            action.goal
                        );

                    foreach (Blueprint blueprint in matchingBlueprints)
                    {
                        if (blueprint.craftingStation != null)
                        {
                            updateAction = new Action(updateAction); //make item one all required items are satisfied
                            updateAction.Init(ActionType.Make_Blueprint_From_Station, blueprint);

                            updateAction = new Action(updateAction);
                            updateAction.Init(ActionType.Move_To_Location, blueprint);
                        }

                        Action bluePrintAction = new Action(updateAction); //make item one all required items are satisfied
                        bluePrintAction.Init(ActionType.Make_Blueprint_From_Inventory, blueprint);

                        foreach (
                            Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems
                        ) // add required items to subaction
                        {
                            Action bluePrintSubAction = new Action(bluePrintAction, true); //each item in subaction
                            bluePrintSubAction.Init(
                                ActionType.Require_Item_In_Inventory,
                                new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData), //have item is required item
                                itemRequirement.itemData
                            );

                            actionList.Add(bluePrintSubAction);
                        }
                    }
                    break;

                case StatType.Have_Item_Equipped:
                    Debug.Log(
                        "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222"
                    );
                    matchingBlueprints =
                        currentAgent.knowledgeHandler.blueprintRepertoire.GetBlueprintsWithGoalStatType(
                            action.goal
                        );

                    foreach (Blueprint blueprint in matchingBlueprints)
                    {
                        if (blueprint.craftingStation != null)
                        {
                            updateAction = new Action(updateAction); //make item one all required items are satisfied
                            updateAction.Init(ActionType.Make_Blueprint_From_Station, blueprint);

                            updateAction = new Action(updateAction);
                            updateAction.Init(ActionType.Move_To_Location, blueprint);
                        }
                        else
                        {
                            updateAction = new Action(updateAction); //equip item once made
                            updateAction.Init(
                                ActionType.Equip_From_Inventory,
                                new Stat(StatType.Have_Item_In_Inventory, blueprint.craftedItem), //have item is crafted item
                                blueprint.craftedItem,
                                false
                            );

                            updateAction = new Action(updateAction); //make item one all required items are satisfied
                            updateAction.Init(ActionType.Make_Blueprint_From_Inventory, blueprint);
                        }

                        foreach (
                            Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems
                        ) // add required items to subaction
                        {
                            Action bluePrintSubAction = new Action(updateAction, true); //each item in subaction
                            bluePrintSubAction.Init(
                                ActionType.Require_Item_In_Inventory,
                                new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData), //have item is required item
                                itemRequirement.itemData
                            );

                            actionList.Add(bluePrintSubAction);
                        }
                    }
                    break;

                default:
                    Debug.Log(
                        "3333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333"
                    );
                    matchingBlueprints =
                        currentAgent.knowledgeHandler.blueprintRepertoire.GetBlueprintsWithGoalStatType(
                            action.goal
                        );

                    foreach (Blueprint blueprint in matchingBlueprints)
                    {
                        // Create action to either Use or Have the crafted item
                        updateAction = new Action(updateAction); //use item once equipped
                        updateAction.Init(
                            ActionType.Use_Item,
                            new Stat(StatType.Have_Item_Equipped, blueprint.craftedItem),
                            blueprint.craftedItem,
                            false
                        );

                        updateAction = new Action(updateAction); //equip item once made
                        updateAction.Init(
                            ActionType.Equip_From_Inventory,
                            new Stat(StatType.Have_Item_In_Inventory, blueprint.craftedItem), //have item is crafted item
                            blueprint.craftedItem,
                            false
                        );

                        if (blueprint.craftingStation != null)
                        {
                            updateAction = new Action(updateAction); //make item one all required items are satisfied
                            updateAction.Init(ActionType.Make_Blueprint_From_Station, blueprint);

                            ///
                            ///
                            ///
                            if (blueprint.requiredTool != null)
                            {
                                updateAction = new Action(updateAction); //each item in subaction
                                updateAction.Init(
                                    ActionType.Equip_From_Inventory,
                                    new Stat(
                                        StatType.Have_Item_In_Inventory,
                                        blueprint.requiredTool
                                    ), //have item is required item
                                    blueprint.requiredTool
                                );
                            }
                            ///



                            updateAction = new Action(updateAction);
                            updateAction.Init(ActionType.Move_To_Location, blueprint);
                        }
                        else
                        {
                            updateAction = new Action(updateAction); //make item one all required items are satisfied
                            updateAction.Init(ActionType.Make_Blueprint_From_Inventory, blueprint);

                            if (blueprint.requiredTool != null)
                            {
                                updateAction = new Action(updateAction); //each item in subaction
                                updateAction.Init(
                                    ActionType.Equip_From_Inventory,
                                    new Stat(
                                        StatType.Have_Item_In_Inventory,
                                        blueprint.requiredTool
                                    ), //have item is required item
                                    blueprint.requiredTool
                                );
                            }
                        }

                        if (blueprint.requiredItems.Count == 0)
                        {
                            updateAction.canComplete = true;
                            actionList.Add(updateAction);
                        }

                        foreach (
                            Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems
                        ) // add required items to subaction
                        {
                            Action bluePrintSubAction = new Action(updateAction, true); //each item in subaction
                            bluePrintSubAction.Init(
                                ActionType.Require_Item_In_Inventory,
                                new Stat(StatType.Have_Item_In_Inventory, itemRequirement.itemData), //have item is required item
                                itemRequirement.itemData
                            );

                            actionList.Add(bluePrintSubAction);
                        }

                        if (blueprint.requiredTool != null)
                        {
                            updateAction = new Action(updateAction, true); //each item in subaction
                            updateAction.Init(
                                ActionType.Require_Item_In_Inventory,
                                new Stat(StatType.Have_Item_In_Inventory, blueprint.requiredTool), //have item is required item
                                blueprint.requiredTool
                            );
                            actionList.Add(updateAction);
                        }
                    }

                    break;
            }
            SaveMasterActionToFile("Pizza.json");
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

            Debug.Log("json data : " + jsonData);

            // Save the JSON data to a file
            File.WriteAllText("Assets/" + filePath, jsonData);

            Debug.Log("MasterAction saved to file: " + filePath);
        }
        #endregion
    }
}
