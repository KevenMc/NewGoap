using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class JSONAction
    {
        public string actionName;
        public float actionCost;
        public string childAction;
        public string[] subActions;

        public JSONAction(Action action)
        {
            this.actionName = action.actionName;
            this.actionCost = action.actionCost;
            if (action.childActions.Count > 0)
            {
                this.childAction = JsonUtility.ToJson(new JSONAction(action.childActions[0]));
            }
            if (action.subActions.Count > 0)
            {
                string[] subActions = new string[action.subActions.Count];
                int x = 0;
                foreach (Action subAction in action.subActions)
                {
                    subActions[0] = JsonUtility.ToJson(new JSONAction(subAction));
                    x++;
                }
                this.subActions = subActions;
            }
        }
    }
}
