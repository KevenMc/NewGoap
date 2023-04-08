using System;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class ActionPlanner : MonoBehaviour
    {
        private Agent agent;
        public List<Action> actionPlans = new List<Action>();
        public Action initialActionPlan;
        public float moveDistance = 1f;
        public int x = 0;
        public int y = 100;

        private void Start()
        {
            agent = GetComponent<Agent>();
        }

        public void SetGoal(Stat goal)
        {
            actionPlans.Clear();
            actionPlans.Add(new Action(goal));
            initialActionPlan = actionPlans[0];

            agent.currentGoal = goal.statType.ToString();
        }

        public void SortPlans(List<Action> actionList)
        {
            actionList.Sort((action1, action2) => action1.actionCost.CompareTo(action2.actionCost));
        }

        #region

        public void ExtendAction(Action action)
        {
            ExtendActionPlanFromInventory(action);
            ExtendActionPlanFromItemMemory(action);
            ExtendActionPlanFromBlueprintRepertoire(action);
            actionPlans.Remove(action);
        }

        private void ExtendActionPlanFromInventory(Action action)
        {
            List<ItemSO> inventoryItems = agent.inventory.returnGoalItems(action.goal.statType);
            foreach (ItemSO itemData in inventoryItems)
            {
                Action newInventoryAction = new Action(action);
                newInventoryAction.Init(ActionType.UseItem, itemData);
                actionPlans.Add(newInventoryAction);
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
                        ActionType.UseItem,
                        new Stat(StatType.HaveItem, item.itemData),
                        item.itemData
                    );
                }

                updateAction = new Action(updateAction);
                updateAction.Init(
                    ActionType.CollectItem,
                    new Stat(StatType.IsAtLocation, item.itemData),
                    item,
                    isAtLocation
                );

                if (!isAtLocation)
                {
                    updateAction = new Action(updateAction);
                    updateAction.Init(
                        ActionType.MoveToLocation,
                        new Stat(StatType.IsAtLocation, item.itemData),
                        item.transform.position,
                        true
                    );
                }

                actionPlans.Add(updateAction);
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
                Action blueprintAction = new Action(action);
                blueprintAction.Init(ActionType.Blueprint, blueprint);

                foreach (Blueprint.ItemRequirement itemRequirement in blueprint.requiredItems)
                {
                    Stat itemRequirementStat = new Stat(
                        StatType.HaveItem,
                        itemRequirement.itemData
                    );
                    Action itemAction = new Action(itemRequirementStat);
                    itemAction.Init(ActionType.BlueprintItem, itemRequirement.itemData);
                    blueprintAction.subActions.Add(itemAction);

                    actionPlans.Add(itemAction);
                }
            }
        }

        #endregion

        public static float GetDistance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        public static float GetDistance(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }
    }
}
