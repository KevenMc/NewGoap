using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class ActionPlanner : MonoBehaviour
    {
        private Agent agent;
        public List<Plan> plans = new List<Plan>();
        public float moveDistance = 1f;
        public Plan initialPlan;

        private void Start()
        {
            agent = GetComponent<Agent>();
        }

        public void SetGoal(Stat goal)
        {
            plans.Clear();
            plans.Add(new Plan(goal));
            initialPlan = plans[0];

            agent.currentGoal = goal.statType.ToString();
        }

        public void SortPlans(List<Plan> planList)
        {
            planList.Sort((plan1, plan2) => plan1.planCost.CompareTo(plan2.planCost));
        }

        #region
        public void ExpandPlan(Plan updatePlan, List<Plan> planList)
        {
            ExpandPlanFromInventory(updatePlan, planList);
            ExpandPlanFromItemMemory(updatePlan, planList);
            ExpandPlanFromBlueprintRepertoire(updatePlan, planList);
            planList.Remove(updatePlan);
        }

        public Plan GeneratePlan()
        {
            return UpdatePlanList(plans);
        }

        private Plan UpdatePlanList(List<Plan> planList)
        {
            Plan updatePlan = GetCheapestPlan(planList);
            if (updatePlan.isComplete)
                return updatePlan;
            if (updatePlan.actions.Count > 0)
            {
                Action lastAction = updatePlan.actions.Last();
                if (lastAction.subPlanLists.Count > 0)
                {
                    foreach (List<Plan> subPlans in lastAction.subPlanLists.Values)
                    {
                        return UpdatePlanList(lastAction.subPlans);
                    }
                }
                else
                {
                    ExpandPlan(updatePlan, planList);
                    return UpdatePlanList(planList);
                }
            }
            else
            {
                ExpandPlan(updatePlan, planList);
                return UpdatePlanList(planList);
            }
            return new Plan();
        }

        public Plan GetCheapestPlan(List<Plan> planList)
        {
            if (planList.Count > 0)
            {
                SortPlans(planList);
                return planList[0];
            }
            return initialPlan;
        }

        private Plan GetSubplans(Plan updatePlan)
        {
            if (updatePlan.actions.Count > 0)
            {
                Action lastAction = updatePlan.actions.Last();
                if (lastAction.subPlans.Count > 0) { }
            }
            return updatePlan;
        }

        public void UpdatePlan(Plan plan)
        {
            plan.CalulateCost();
            Debug.Log("Current plan cost is : " + plan.planCost);
            Plan updatePlan = GetSubplans(plan);
        }

        private void ExpandPlanFromInventory(Plan updatePlan, List<Plan> plans)
        {
            List<ItemSO> inventoryItems = agent.inventory.returnGoalItems(updatePlan.goal);

            foreach (ItemSO item in inventoryItems)
            {
                Action newInventoryAction = new Action(ActionType.UseItem);
                newInventoryAction.Setup(
                    "Use " + item.itemName,
                    1, // Set the action cost appropriately
                    updatePlan.goal,
                    item,
                    null,
                    Vector3.zero,
                    true
                );

                Plan newPlan = new Plan(updatePlan, newInventoryAction);
                plans.Add(newPlan);
            }
        }

        private void ExpandPlanFromItemMemory(Plan updatePlan, List<Plan> plans)
        {
            List<Item> memoryItems = agent.knowledgeHandler.itemMemory.returnGoalItems(
                updatePlan.goal
            );

            foreach (Item item in memoryItems)
            {
                Plan newPlan = new Plan();

                if (updatePlan.goal.statType != StatType.Item)
                {
                    Action newInventoryAction = new Action(ActionType.UseItem);
                    newInventoryAction.Setup(
                        "Use " + item.itemData.itemName,
                        1, // Set the action cost appropriately
                        updatePlan.goal,
                        item.itemData,
                        item,
                        Vector3.zero,
                        true
                    );

                    Action newCollectAction = new Action(ActionType.CollectItem);
                    newCollectAction.Setup(
                        "Collect " + item.itemData.itemName,
                        1, // Set the action cost appropriately
                        updatePlan.goal,
                        item.itemData,
                        item,
                        item.transform.position,
                        true
                    );

                    newPlan = new Plan(updatePlan, newInventoryAction);
                    newPlan = new Plan(newPlan, newCollectAction);
                }
                else
                {
                    Action newCollectAction = new Action(ActionType.CollectItem);
                    newCollectAction.Setup(
                        "Collect " + item.itemData.itemName,
                        1, // Set the action cost appropriately
                        updatePlan.goal,
                        item.itemData,
                        item,
                        item.transform.position,
                        true
                    );

                    newPlan = new Plan(updatePlan, newCollectAction);
                }

                if (
                    agent.distanceToArrive
                    <= GetDistance(transform.position, item.transform.position)
                )
                {
                    Action newMoveToAction = new Action(ActionType.MoveToLocation);
                    newMoveToAction.Setup(
                        "Move to " + item.itemData.itemName,
                        0, // Set the action cost appropriately
                        updatePlan.goal,
                        null,
                        null,
                        item.transform.position,
                        true
                    );

                    newPlan = new Plan(newPlan, newMoveToAction);
                }

                plans.Add(newPlan);
            }
        }

        public void ExpandPlanFromBlueprintRepertoire(Plan updatePlan, List<Plan> plans)
        {
            List<Blueprint> matchingBlueprints = new List<Blueprint>();
            foreach (
                Blueprint blueprint in agent.knowledgeHandler.blueprintRepertoire.GetBlueprintsWithGoalStatType(
                    updatePlan.goal
                )
            )
            {
                Plan newPlan = updatePlan;

                Action newInventoryAction = new Action(ActionType.UseItem);
                newInventoryAction.Setup(
                    "Use " + blueprint.craftedItem.itemName,
                    1, // Set the action cost appropriately
                    updatePlan.goal,
                    blueprint.craftedItem,
                    null,
                    Vector3.zero,
                    false
                );

                newPlan = new Plan(newPlan, newInventoryAction);
                Debug.Log("Adding blueprint");

                Action newBlueprintAction = new Action(ActionType.Blueprint);
                newBlueprintAction.Setup(
                    "Blueprint " + blueprint.blueprintName,
                    0, // Set the action cost appropriately
                    updatePlan.goal,
                    blueprint.craftedItem,
                    blueprint,
                    false
                );
                newBlueprintAction.GenerateSubPlans();

                newPlan = new Plan(newPlan, newBlueprintAction);
                Debug.Log("Action has subplans? : " + newBlueprintAction.subPlans.Count);
                plans.Add(newPlan);
            }

            //return matchingBlueprints;
            return;
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
