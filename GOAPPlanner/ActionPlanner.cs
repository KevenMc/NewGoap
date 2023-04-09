using System;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

namespace GOAP
{
    public class ActionPlanner : MonoBehaviour
    {
        private Agent agent;
        public List<Action> actionList = new List<Action>();
        public Action masterAction;
        public float moveDistance = 1f;
        public int x = 0;
        public int y = 100;

        private void Start()
        {
            agent = GetComponent<Agent>();
        }

        public void SetGoal(Stat goal)
        {
            Debug.Log("Setting goal");
            actionList.Clear();
            actionList.Add(new Action(goal));
            masterAction = actionList[0];
            agent.currentGoal = goal.statType.ToString();

            RecursiveFindAction();
            SaveMasterActionToFile("MYaction.json");
        }

        public void RecursiveFindAction()
        {
            if (actionList.Count == 0)
            {
                Debug.Log("No possible plan of action can be found");
                return;
            }
            SortPlans(actionList);
            if (actionList[0].CanComplete())
            {
                Debug.Log("-------------------------------------------");
                if (CanCompleteMasterAction(masterAction))
                {
                    RecursiveShowActions(actionList[0]);
                }
                else
                {
                    Debug.Log("No, this will not do");
                }
                return;
            }
            ExtendAction(actionList[0]);
            RecursiveFindAction();
        }

        public Boolean CanCompleteMasterAction(Action action)
        {
            Debug.Log("Checking action : " + action.actionName);
            if (action.subActions.Count > 0)
            {
                SortPlans(action.subActions);
                CanCompleteMasterAction(action.subActions[0]);
            }
            else if (action.childActions.Count > 0)
            {
                //handle child actions
                CanCompleteMasterAction(action.childActions.Last());
            }
            else if (!action.canComplete)
            {
                return false;
            }
            return true;
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
            Debug.Log("Extending current action");
            ExtendActionPlanFromInventory(action);
            ExtendActionPlanFromItemMemory(action);
            ExtendActionPlanFromBlueprintRepertoire(action);
            Debug.Log("Remove actions from actionPlans");

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
            List<ItemSO> inventoryItems = agent.inventory.returnGoalItems(action.goal.statType);
            foreach (ItemSO itemData in inventoryItems)
            {
                Action newInventoryAction = new Action(action);
                newInventoryAction.Init(ActionType.Use_Item, action.goal, itemData, true);
                actionList.Add(newInventoryAction);
            }
        }

        private void ExtendActionPlanFromItemMemory(Action action)
        {
            List<Item> memoryItems = agent.knowledgeHandler.itemMemory.returnGoalItems(action.goal);

            foreach (Item item in memoryItems)
            {
                Boolean isAtLocation =
                    agent.distanceToArrive
                    >= GetDistance(transform.position, item.transform.position);

                Action updateAction = action;
                if (action.goal.statType != StatType.HaveItem)
                {
                    updateAction = new Action(updateAction);
                    updateAction.Init(
                        ActionType.Use_Item,
                        new Stat(StatType.HaveItem, item.itemData),
                        item.itemData
                    );
                }

                updateAction = new Action(updateAction);
                updateAction.Init(
                    ActionType.Collect_Item,
                    new Stat(StatType.IsAtLocation, item.itemData),
                    item,
                    isAtLocation
                );

                if (!isAtLocation)
                {
                    updateAction = new Action(updateAction);
                    updateAction.Init(
                        ActionType.Move_To_Location,
                        new Stat(StatType.IsAtLocation, item.itemData),
                        item.transform.position,
                        true
                    );
                }

                actionList.Add(updateAction);
            }
        }

        public void ExtendActionPlanFromBlueprintRepertoire(Action action)
        {
            List<Blueprint> matchingBlueprints = new List<Blueprint>();
            foreach (
                Blueprint blueprint in agent.knowledgeHandler.blueprintRepertoire.GetBlueprintsWithGoalStatType(
                    action.goal
                )
            )
            {
                if (blueprint.craftedItem != null)
                {
                    action = new Action(action);
                    action.Init(ActionType.Use_Item, action.goal, blueprint.craftedItem);
                }

                Action blueprintAction = new Action(action);
                blueprintAction.Init(ActionType.Blueprint_Make, blueprint);

                foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems)
                {
                    Stat itemRequirementStat = new Stat(
                        StatType.HaveItem,
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

        public void SortPlans(List<Action> actionList)
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
            // Create a dictionary to hold the masterAction and its sub-actions
            Dictionary<string, object> actionData = new Dictionary<string, object>();

            // Add the masterAction to the dictionary
            actionData.Add("MasterAction", masterAction.actionName);
            Debug.Log("MasterAction : " + masterAction.actionName);
            JSONAction jSONAction = new JSONAction(masterAction);
            // Debug.Log(jSONAction);


            // Convert the dictionary to JSON
            string jsonData = JsonUtility.ToJson(jSONAction);
            jsonData = jsonData
                .Replace("\n", "")
                .Replace("\\", "")
                .Replace("\"{", "{")
                .Replace("}\"", "}");
            //  .Replace("}\"", "}");

            Debug.Log("json data : " + jsonData);

            // Save the JSON data to a file
            File.WriteAllText(filePath, jsonData);

            Debug.Log("MasterAction saved to file: " + filePath);
        }
        #endregion
    }
}
