using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class PlanHandler : MonoBehaviour
    {
        public ActionPlanner actionPlanner;

        public Plan masterPlan;
        public Plan currentParentPlan = null;
        public Plan currentPlan;
        public List<Action> currentActionList = new List<Action>();
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
            masterPlan = actionPlanner.GeneratePlan();
            // Generate the plan using ExpandPlan

            return masterPlan;
        }

        public void SetActionList()
        {
            currentActionList.Clear();
            RecursiveSetActionList(masterPlan);
            Debug.Log(
                "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$"
            );
            foreach (Action action in currentActionList)
            {
                Debug.Log(action.actionName);
            }
            Debug.Log(
                "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$"
            );
        }

        public void RecursiveSetActionList(Plan thisPlan)
        {
            currentActionList.AddRange(thisPlan.actions);
            if (thisPlan.hasSubPlans)
            {
                Debug.Log(
                    "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$"
                );
                List<Plan> subPlans = new List<Plan>();
                foreach (List<Plan> plans in thisPlan.actions.Last().subPlanLists.Values)
                {
                    Debug.Log(plans[0].goal.statType);
                    currentActionList.AddRange(plans[0].actions);
                    RecursiveSetActionList(plans[0]);
                }
            }
        }
    }
}
