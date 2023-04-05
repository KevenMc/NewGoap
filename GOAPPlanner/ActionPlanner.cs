using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class ActionPlanner : MonoBehaviour
    {
        private Agent agent;
        public List<Plan> plans = new List<Plan>();
        public Plan currentPlan;
        public Action currentAction;
        public string currentActionName;
        public ReverseIterate<Action> reverseActions;
        public int currentActionIndex;

        private void Start()
        {
            agent = GetComponent<Agent>();
        }

        public void SetGoal(Stat goal)
        {
            plans.Clear();
            plans.Add(new Plan(goal));
            agent.currentGoal = goal.statType.ToString();
        }

        public void ShowCurrentPlan()
        {
            if (plans.Count > 0)
            {
                plans[0].ShowPlanContents();
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog("No Plans!", null, "Close");
            }
        }

        public void SetCurrentPlan()
        {
            Plan updatePlan = plans.OrderBy(p => p.planCost).FirstOrDefault();
            if (!updatePlan.isComplete)
            {
                currentPlan = updatePlan;
                Debug.Log(currentPlan.actions[0]);
                reverseActions = new ReverseIterate<Action>(currentPlan.actions);
                currentAction = reverseActions.Next();
                currentActionName = currentAction.actionName;
            }
        }

        public void ExecuteCurrentPlan()
        {
            ReverseIterate<Action> reverseActions = new ReverseIterate<Action>(currentPlan.actions);
        }

        public void UpdatePlan(Plan updatePlan)
        {
            //check inventory
            List<ItemSO> updateItems = agent.inventory.returnGoalItems(updatePlan.goal);

            foreach (ItemSO item in updateItems)
            {
                Action newInventoryAction = new Action(item);
                Plan newPlan = new Plan(updatePlan, newInventoryAction);
                plans.Add(newPlan);
            }

            //check itemmemory
            List<Item> memoryItems = agent.knowledgeHandler.itemMemory.returnGoalItems(
                updatePlan.goal
            );

            foreach (Item item in memoryItems)
            {
                Action newInventoryAction = new Action(item.itemData);
                Plan newPlan = new Plan(updatePlan, newInventoryAction);

                Action newCollectAction = new Action(item.itemData, item);
                newPlan = new Plan(newPlan, newCollectAction);
                plans.Add(newPlan);
            }

            plans.Remove(updatePlan);
        }

        private bool CanUseItem(ItemSO item, Stat goal)
        {
            foreach (var statEffect in item.statEffects)
            {
                // Check if the item has a stat effect that can be used towards the goal
                if (goal.statType == statEffect.statType && statEffect.value < 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void CreateSubPlans(
            Action parentAction,
            Stat currentGoal,
            List<Stat> remainingGoals
        )
        {
            // If there are no more goals, return
            if (remainingGoals.Count == 0)
            {
                return;
            }

            // Get the first remaining goal and create a new sub-plan for it
            Stat nextGoal = remainingGoals[0];
            Plan subPlan = new Plan(nextGoal);

            // Add the sub-plan to the parent plan and set it as the current plan
            parentAction.AddSubPlan(subPlan);
            Plan currentPlan = subPlan;

            // Loop through the items in the agent's inventory and try to use them towards the sub-goal
            foreach (var inventoryItem in agent.inventory.items)
            {
                ItemSO item = inventoryItem.item;

                // Check if the item can be used towards the sub-goal
                if (CanUseItem(item, nextGoal))
                {
                    // Create a new action for using the item
                    Action useItemAction = new Action(item);

                    // Add the action to the current plan
                    currentPlan.AddAction(useItemAction);
                    CreateSubPlans(useItemAction, nextGoal, remainingGoals.Skip(1).ToList());
                }
            }
        }
    }
}
