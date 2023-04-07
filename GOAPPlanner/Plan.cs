using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class Plan
    {
        public List<Action> actions = new List<Action>();
        public Stat goal;
        public Stat endGoal;
        public float planCost = 0;
        public bool isComplete = false;

        public Plan() { }

        public Plan(Stat goal)
        {
            this.endGoal = goal;
            this.goal = goal;
        }

        public Plan(Plan originalPlan, Action newAction)
        {
            this.endGoal = originalPlan.endGoal;
            this.goal = newAction.goal;
            this.actions = new List<Action>(originalPlan.actions);
            this.AddAction(newAction);
            this.CalulateCost();
        }

        public void AddAction(Action action)
        {
            actions.Add(action);
            if (action.canComplete)
            {
                isComplete = true;
                planCost = -1;
            }
        }

        public void CalulateCost()
        {
            float currentCost = 0f;
            foreach (Action action in actions)
            {
                foreach (Plan subplan in action.subPlans)
                {
                    subplan.CalulateCost();
                }
                currentCost += action.TotalActionCost();
            }
            planCost = currentCost;
        }

        public void ShowPlanContents()
        {
            string planContents = "Plan contents:\n\n";
            planContents += "EndGoal : " + endGoal.statType.ToString() + "\n\n";

            foreach (Action action in actions)
            {
                planContents += "- " + action.actionName + "\n";
            }

            UnityEditor.EditorUtility.DisplayDialog("Plan contents", planContents, "Close");
        }
    }
}
