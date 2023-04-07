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

        // public void ExpandPlan(Plan updatePlan)
        // {
        //     //check inventory
        //     List<ItemSO> updateItems = agent.inventory.returnGoalItems(updatePlan.goal);

        //     foreach (ItemSO item in updateItems)
        //     {
        //         Action newInventoryAction = new Action(item, true);
        //         Plan newPlan = new Plan(updatePlan, newInventoryAction);
        //         plans.Add(newPlan);
        //     }

        //     //check item memory
        //     List<Item> memoryItems = agent.knowledgeHandler.itemMemory.returnGoalItems(
        //         updatePlan.goal
        //     );

        //     foreach (Item item in memoryItems)
        //     {
        //         Action newInventoryAction = new Action(item.itemData);
        //         Plan newPlan = new Plan(updatePlan, newInventoryAction);

        //         Action newCollectAction = new Action(item.itemData, item);
        //         newPlan = new Plan(newPlan, newCollectAction);

        //         if (moveDistance <= GetDistance(transform.position, item.transform.position))
        //         {
        //             Debug.Log((GetDistance(transform.position, item.transform.position)));
        //             Action newMoveToAction = new Action(
        //                 item.transform.position,
        //                 item.itemData.itemName,
        //                 true
        //             );
        //             newPlan = new Plan(newPlan, newMoveToAction);
        //         }
        //         plans.Add(newPlan);
        //     }
        //     plans.Remove(updatePlan);
        // }
        public void SortPlans()
        {
            plans.Sort((plan1, plan2) => plan1.planCost.CompareTo(plan2.planCost));
        }

        #region
        public void ExpandPlan(Plan updatePlan)
        {
            // Check inventory
            List<ItemSO> inventoryItems = agent.inventory.returnGoalItems(updatePlan.goal);

            foreach (ItemSO item in inventoryItems)
            {
                Action newInventoryAction = new Action(ActionType.UseItem);
                newInventoryAction.Setup(
                    "Use " + item.itemName,
                    0, // Set the action cost appropriately
                    updatePlan.goal,
                    item,
                    null,
                    Vector3.zero,
                    true
                );

                Plan newPlan = new Plan(updatePlan, newInventoryAction);
                plans.Add(newPlan);
            }

            // Check item memory
            List<Item> memoryItems = agent.knowledgeHandler.itemMemory.returnGoalItems(
                updatePlan.goal
            );

            foreach (Item item in memoryItems)
            {
                Action newInventoryAction = new Action(ActionType.UseItem);
                newInventoryAction.Setup(
                    "Use " + item.itemData.itemName,
                    0, // Set the action cost appropriately
                    updatePlan.goal,
                    item.itemData,
                    item,
                    Vector3.zero,
                    true
                );

                Action newCollectAction = new Action(ActionType.CollectItem);
                newCollectAction.Setup(
                    "Collect " + item.itemData.itemName,
                    0, // Set the action cost appropriately
                    updatePlan.goal,
                    item.itemData,
                    item,
                    item.transform.position,
                    true
                );

                Plan newPlan = new Plan(updatePlan, newInventoryAction);
                newPlan = new Plan(newPlan, newCollectAction);

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
            plans.Remove(updatePlan);
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
