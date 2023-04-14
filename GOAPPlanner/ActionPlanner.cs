using System.Data;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

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
            Debug.Log(goal.statType.ToString());
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
                RecursiveShowActions(actionList[0]);
                SaveMasterActionToFile("json.json");
                currentAgent.requiresNewAction = false;
                return true;
            }

            // Parallel.ForEach(
            //     actionList,
            //     action =>
            //     {
            //         ExtendAction(action);
            //     }
            // );

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
                    returnList.Add(CanCompleteMasterAction(subAction));
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
            ExtendActionPlanFromStationMemory(action);
            ExtendActionPlanFromBlueprintRepertoire(action);

            if (action.childActions.Count == 0 && !action.CanComplete())
            {
                RecursiveDropActions(action);
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
                action.goal.statType
            );
            foreach (ItemSO itemData in inventoryItems)
            {
                Action newInventoryAction = new Action(action);
                newInventoryAction.Init(ActionType.Use_Item, action.goal, itemData, false);

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

        private void ExtendActionPlanFromItemMemory(Action action)
        {
            List<Item> memoryItems = currentAgent.knowledgeHandler.itemMemory.ReturnGoalItems(
                action.goal
            );

            foreach (Item item in memoryItems)
            {
                Boolean isAtLocation =
                    currentAgent.distanceToArrive >= GetDistance(currentLocation, item.location);

                Action updateAction = action;
                if (action.goal.statType != StatType.Have_Item_Equipped)
                {
                    updateAction = new Action(updateAction);
                    updateAction.Init(
                        ActionType.Use_Item,
                        new Stat(StatType.Have_Item_Equipped, item.itemData),
                        item.itemData
                    );

                    updateAction = new Action(updateAction);
                    updateAction.Init(
                        ActionType.Collect_And_Equip,
                        new Stat(StatType.Move_To_Location, item.itemData),
                        item,
                        isAtLocation
                    );
                }
                else
                {
                    updateAction = new Action(updateAction);
                    updateAction.Init(
                        ActionType.UnEquip_To_Inventory,
                        new Stat(StatType.Have_Item_Equipped, item.itemData),
                        item,
                        false
                    );

                    updateAction = new Action(updateAction);
                    updateAction.Init(
                        ActionType.Collect_And_Equip,
                        new Stat(StatType.Move_To_Location, item.itemData),
                        item,
                        isAtLocation
                    );
                }

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

        private void ExtendActionPlanFromStationMemory(Action action)
        {
            List<Station> memoryStations =
                currentAgent.knowledgeHandler.stationMemory.ReturnGoalStations(action.goal);

            foreach (Station station in memoryStations)
            {
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
            List<Blueprint> matchingBlueprints = new List<Blueprint>();
            foreach (
                Blueprint blueprint in currentAgent.knowledgeHandler.blueprintRepertoire.GetBlueprintsWithGoalStatType(
                    action.goal
                )
            )
            {
                Debug.Log(
                    "Searching for blueprint recipe : "
                        + blueprint
                        + " : -------------------------------------------------------------"
                );
                if (action.actionType != ActionType.Blueprint_Require_Item)
                {
                    action = new Action(action);
                    action.Init(ActionType.Use_Item, action.goal, blueprint.craftedItem);

                    action = new Action(action);
                    action.Init(
                        ActionType.Equip_From_Inventory,
                        action.goal,
                        blueprint.craftedItem
                    );
                }

                Action blueprintAction = new Action(action);
                blueprintAction.Init(ActionType.Make_Blueprint_From_Inventory, blueprint);

                foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems)
                {
                    Stat itemRequirementStat = new Stat(
                        StatType.Have_Item_Equipped,
                        itemRequirement.itemData
                    );

                    Action itemAction = new Action(itemRequirementStat);
                    itemAction.Init(
                        ActionType.Blueprint_Require_Item,
                        itemRequirement.itemData,
                        blueprintAction
                    );

                    blueprintAction.subActions.Add(itemAction);
                    itemAction.isSubAction = true;
                    actionList.Add(itemAction);
                }
            }
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
            File.WriteAllText(filePath, jsonData);

            Debug.Log("MasterAction saved to file: " + filePath);
        }
        #endregion
    }
}
