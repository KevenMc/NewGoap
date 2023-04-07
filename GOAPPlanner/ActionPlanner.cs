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
        public int x = 0;
        public int y = 100;

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
            Debug.Log("Now removing a plan : " + updatePlan.goal.statType);
            planList.Remove(updatePlan);
        }

        public Plan GeneratePlan()
        {
            Debug.Log("My plan list is this long : " + plans.Count);
            return UpdatePlanList(plans);
        }

        private Plan UpdatePlanList(List<Plan> planList)
        {
            Debug.Log("My plan list is this long : " + planList.Count);
            if (planList.Count == 0)
            {
                Debug.Log("No more plans to make");
                return new Plan();
            }
            if (x == y)
            {
                Debug.Log("recursion is not your friend");
                return new Plan();
            }
            x++;
            Plan updatePlan = GetCheapestPlan(planList);
            if (updatePlan.isComplete)
                return updatePlan;
            if (updatePlan.actions.Count > 0)
            {
                Debug.Log(1);
                Action lastAction = updatePlan.actions.Last();
                planList.Remove(updatePlan);
                if (lastAction.subPlanLists.Count > 0)
                {
                    Debug.Log(2);
                    foreach (List<Plan> subPlans in lastAction.subPlanLists.Values)
                    {
                        Debug.Log(3);
                        foreach (Stat key in lastAction.subPlanLists.Keys)
                        {
                            if (lastAction.subPlanLists[key].Count == 0)
                            {
                                return new Plan();
                            }
                            Plan subPlan = UpdatePlanList(lastAction.subPlanLists[key]);
                            if (!subPlan.isComplete)
                            {
                                return new Plan();
                            }
                        }
                        return updatePlan;
                        // return UpdatePlanList(lastAction.subPlans);
                    }
                }
                else
                {
                    Debug.Log(4);
                    Debug.Log("vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv");
                    foreach (Action action in updatePlan.actions)
                    {
                        Debug.Log(action.actionName);
                    }
                    Debug.Log(updatePlan.goal.statType);
                    Debug.Log("sorry, what ? " + updatePlan.endGoal.statType);

                    Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                    ExpandPlan(updatePlan, planList);

                    return UpdatePlanList(planList);
                }
            }
            else
            {
                Debug.Log(5);
                Debug.Log("vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*");

                foreach (Action action in updatePlan.actions)
                {
                    Debug.Log(action.actionName);
                }
                Debug.Log(updatePlan.endGoal.statType);
                Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*");

                ExpandPlan(updatePlan, planList);

                return UpdatePlanList(planList);
            }
            Debug.Log(6);
            return new Plan();
        }

        public Plan GetCheapestPlan(List<Plan> planList)
        {
            if (planList.Count > 0)
            {
                SortPlans(planList);
                Debug.Log(
                    "Now looking at plan with endgoal of : "
                        + planList[0].endGoal.statType
                        + " and current goal of : "
                        + planList[0].goal.statType
                );
                return planList[0];
            }
            return initialPlan;
        }

        private Plan GetSubplans(Plan updatePlan)
        {
            if (updatePlan.actions.Count > 0)
            {
                Debug.Log(
                    "Trying to get subplans from this plan which has a goal of "
                        + updatePlan.goal.statType
                );
                Action lastAction = updatePlan.actions.Last();
                foreach (Stat key in lastAction.subPlanLists.Keys)
                {
                    Debug.Log(
                        "£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££"
                    );
                    Debug.Log(
                        "Number of subplans for goal "
                            + key.statType
                            + " is "
                            + lastAction.subPlanLists[key].Count
                    );
                    Debug.Log(key);
                }
                if (lastAction.subPlanLists.Count > 0)
                {
                    Debug.Log(
                        "£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££"
                    );
                }
            }
            return updatePlan;
        }

        public void UpdatePlan(Plan plan)
        {
            plan.CalulateCost();
            Plan updatePlan = GetSubplans(plan);
        }

        private void ExpandPlanFromInventory(Plan updatePlan, List<Plan> planList)
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
                Debug.Log("Newplan 1");
                planList.Add(newPlan);
            }
        }

        private void ExpandPlanFromItemMemory(Plan updatePlan, List<Plan> planList)
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
                Debug.Log("Newplan 2");

                planList.Add(newPlan);
            }
        }

        public void ExpandPlanFromBlueprintRepertoire(Plan updatePlan, List<Plan> planList)
        {
            int p = 0;
            List<Blueprint> matchingBlueprints = new List<Blueprint>();
            foreach (
                Blueprint blueprint in agent.knowledgeHandler.blueprintRepertoire.GetBlueprintsWithGoalStatType(
                    updatePlan.goal
                )
            )
            {
                p++;
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

                Stat blueprintGoal = new Stat(StatType.Blueprint, 0, 0, 0, 1);
                Action newBlueprintAction = new Action(ActionType.Blueprint);
                newBlueprintAction.Setup(
                    "Blueprint " + blueprint.blueprintName,
                    0, // Set the action cost appropriately
                    blueprintGoal,
                    blueprint.craftedItem,
                    blueprint,
                    false
                );
                newBlueprintAction.GenerateSubPlans();

                newPlan = new Plan(newPlan, newBlueprintAction);
                Debug.Log(newBlueprintAction.subPlanLists.Count);
                Debug.Log("Newplan 3");

                foreach (Stat key in newPlan.actions.Last().subPlanLists.Keys)
                {
                    Debug.Log(key.statType);
                }
                Debug.Log("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                Debug.Log(newPlan.goal.statType);
                plans.Add(newPlan);
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
