using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class JSONAction
    {
        public string actionName;
        public string childAction;
        public bool canComplete;

    //     public JSONAction(Action action)
    //     {
    //         this.actionName = action.actionName;
    //         this.canComplete = action.canComplete;

    //         if (action.childAction?.subActions.Count > 0)
    //         {
    //             this.childAction = JsonUtility.ToJson(
    //                 new JSONActionWithSubAction(action.childAction)
    //             );
    //         }
    //         else if (action.childAction?.childAction != null)
    //         {
    //             this.childAction = JsonUtility.ToJson(new JSONAction(action.childAction));
    //         }
    //     }
    // }

    // public class JSONActionEndNode
    // {
    //     public string actionName;
    //     public bool canComplete;

    //     public JSONActionEndNode(Action action)
    //     {
    //         this.actionName = action.actionName;
    //         this.canComplete = action.canComplete;
    //     }
    // }

    // public class JSONActionWithSubAction
    // {
    //     public string actionName;
    //     public bool canComplete;

    //     public string[] subActions;

    //     public JSONActionWithSubAction(Action action)
    //     {
    //         this.actionName = action.actionName;
    //         this.canComplete = action.canComplete;
    //         string[] subActions = new string[action.subActions.Count];
    //         int i = 0;
    //         foreach (Action subAction in action.subActions)
    //         {
    //             subActions[i] = JsonUtility.ToJson(new JSONAction(subAction));

    //             i++;
    //         }
    //         this.subActions = subActions;
    //     }
    }
}
