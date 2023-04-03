using System.Collections.Generic;
using UnityEngine;

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
        var currentGoal = agent.GetCurrentStatGoal();

        // Get the list of items in the agent's inventory
        var inventoryItems = agent.inventory.items;

        // Sort the items by their value towards the current goal
        inventoryItems.Sort((a, b) => GetItemValue(b.item, currentGoal).CompareTo(GetItemValue(a.item, currentGoal)));

        // Loop through the items and try to use them towards the goal
        foreach (var inventoryItem in inventoryItems)
        {
            var item = inventoryItem.item;

            // Check if the item can be used towards the goal
            if (CanUseItem(item, currentGoal))
            {
                // Use the item and update the agent's stats
                agent.inventory.UseItem(item, agent);
                return;
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

    private int GetItemValue(ItemSO item, Stat goal)
    {
        var value = 0;

        foreach (var statEffect in item.statEffects)
        {
            // Check if the item has a stat effect that can be used towards the goal
            if (goal.statType == statEffect.statType && statEffect.value < 0)
            {
                value += Mathf.RoundToInt(-statEffect.value);
            }
        }

        return value;
    }
}
    
}
