using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class ActionManager : AbstractManager<ActionHandler>
    {
        public static ActionManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected override void PerformTask(ActionHandler subscriber)
        {
            if (subscriber.currentAction != null)
            {
                switch (subscriber.currentAction.actionType)
                {
                    case ActionType.Move_To_Location:
                        if (subscriber.HasArrivedAtLocation())
                        {
                            subscriber.UnregisterActionHandler();
                            subscriber.ExecuteAction();
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
