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
        public int planCost = 0;
        public bool isComplete = false;

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
            this.ShowPlanContents();
        }

        public void AddAction(Action action)
        {
            actions.Add(action);
        }

        private void CalulateCost()
        {
            int currentCost = 0;
            foreach (Action action in actions)
            {
                currentCost += action.actionCost;
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
            // Debug.Log(planContents);
            // Alternatively, show the contents in a box in the Unity Editor window:
            UnityEditor.EditorUtility.DisplayDialog("Plan contents", planContents, "Close");
        }
    }
}
