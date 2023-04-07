using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class PlanHandler : MonoBehaviour
    {
        public ActionPlanner actionPlanner;

        public Plan currentPlan;
        public bool executingCurrentPlan = false;

        private void Awake()
        {
            // Instantiate the ActionPlanner object
            //  actionPlanner = new ActionPlanner();
        }

        public void SetCurrentPlan()
        {
            if (actionPlanner.plans.Count > 0)
            {
                currentPlan = actionPlanner.plans[0];
            }
        }

        public Plan GenerateCompletePlan(Stat goal)
        {
            Debug.Log("Generating new plan for goal : " + goal.statType);
            // Set the goal in the action planner
            actionPlanner.SetGoal(goal);

            // Generate the plan using ExpandPlan
            currentPlan = actionPlanner.initialPlan;
            int x = 0;
            while (currentPlan != null)
            {
                actionPlanner.SortPlans();
                SetCurrentPlan();
                x++;
                if (x > 5)
                {
                    Debug.Log("OH NO!");
                    return new Plan();
                }
                if (currentPlan.isComplete)
                {
                    Debug.Log("Plan has been generated");
                    currentPlan.ShowPlanContents();
                    return currentPlan;
                }
                actionPlanner.ExpandPlan(currentPlan);
            }

            return null;
        }
    }
}
