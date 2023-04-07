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

        public Plan GenerateCompletePlan(Stat goal)
        {
            Debug.Log("Generating new plan for goal : " + goal.statType);
            // Set the goal in the action planner
            actionPlanner.SetGoal(goal);
            currentPlan = actionPlanner.GeneratePlan();
            // Generate the plan using ExpandPlan

            return currentPlan;
        }
    }
}
