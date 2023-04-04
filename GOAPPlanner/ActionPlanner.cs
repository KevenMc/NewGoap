using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class ActionPlanner : MonoBehaviour
    {
        private Agent agent;

        private void Start()
        {
            agent = GetComponent<Agent>();
        }

        public void Plan()
        {
            // Get the current goal from the agent's stat handler
            Stat currentGoal = agent.GetCurrentStatGoal();

            // Create a new plan with the current goal as the root goal
            Plan currentPlan = new Plan(currentGoal);

            // Loop through the items in the agent's inventory and try to use them towards the goal
            foreach (var inventoryItem in agent.inventory.items)
            {
                ItemSO item = inventoryItem.item;

                // Check if the item can be used towards the goal
                if (CanUseItem(item, currentGoal))
                {
                    // Create a new action for using the item
                    Action useItemAction = new Action(item);

                    // Add the action to the current plan
                    currentPlan.AddAction(useItemAction);

                    // Update the agent's stats and add a new sub-plan for the remaining goals
                    agent.inventory.UseItem(item, agent);
                    CreateSubPlans(currentPlan, currentGoal, agent.GetStatGoals());

                    // Return if a solution is found
                    if (currentPlan.IsComplete())
                    {
                        return;
                    }
                }
            }
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
