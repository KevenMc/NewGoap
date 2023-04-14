using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class JSONAction
    {
        public string actionName;
        public string parentAction;
        public string masterAction;
        public float actionCost;
        public string childAction;

        public JSONAction(Action action)
        {
            this.actionName = action.actionName;
            if (action.parentAction != null)
            {
                this.parentAction = action.parentAction.actionName;
            }
            this.masterAction = action.masterAction.actionName;
            this.actionCost = action.actionCost;
            if (action.childActions.Count > 0)
            {
                if (action.childActions[0].subActions.Count > 0)
                {
                    this.childAction = JsonUtility.ToJson(
                        new JSONActionWithSubAction(action.childActions[0])
                    );
                }
                else if (action.childActions[0].childActions.Count > 0)
                {
                    this.childAction = JsonUtility.ToJson(new JSONAction(action.childActions[0]));
                }
                else
                {
                    this.childAction = JsonUtility.ToJson(
                        new JSONActionEndNode(action.childActions[0])
                    );
                }
            }
        }
    }

    public class JSONActionEndNode
    {
        public string actionName;
        public string parentAction;
        public string masterAction;
        public float actionCost;

        public JSONActionEndNode(Action action)
        {
            this.actionName = action.actionName;
            this.parentAction = action.parentAction.actionName;
            this.masterAction = action.masterAction.actionName;
            this.actionCost = action.actionCost;
        }
    }

    public class JSONActionWithSubAction
    {
        public string actionName;
        public string parentAction;
        public string masterAction;
        public float actionCost;
        public string[] subActions;

        public JSONActionWithSubAction(Action action)
        {
            this.actionName = action.actionName;
            this.parentAction = action.parentAction.actionName;
            this.masterAction = action.masterAction.actionName;
            this.actionCost = action.actionCost;
            string[] subActions = new string[action.subActions.Count];
            int i = 0;
            foreach (Action subAction in action.subActions)
            {
                subActions[i] = JsonUtility.ToJson(new JSONAction(subAction));
                i++;
            }
            this.subActions = subActions;
        }
    }
}
