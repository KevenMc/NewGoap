using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class ActionHandler : MonoBehaviour
    {
        public PlanHandler planHandler;
        public Agent agent;
        public MovementHandler movementHandler;
        public Inventory inventory;
        public StatHandler statHandler;
        public Action currentAction;
        public ReverseIterate<Action> actionServer;
        public MoveTo moveTo;
        public bool movingToLocation = false;
        public bool hasCollectedItem = false;
        public bool hasUsedItem = false;
        public bool isExecutingPlan = false;

        public void GetActionServer(Plan plan)
        {
            if (!planHandler.executingCurrentPlan)
            {
                planHandler.executingCurrentPlan = true;
                planHandler.SetActionList();
                actionServer = new ReverseIterate<Action>(planHandler.currentActionList);
                ServeNextAction();
                actionServer.IsLastAction();
            }
        }

        private void ServeNextAction()
        {
            movingToLocation = false;
            hasCollectedItem = false;
            currentAction = actionServer.Next();
        }

        public void ExecuteNextAction()
        {
            ExecuteAction(currentAction);
        }

        private void ExecuteAction(Action action)
        {
            action.actionStatus = ActionStatus.Executing;
            Debug.Log("Executing action : " + action.actionName);

            switch (action.actionType)
            {
                case ActionType.UseItem:
                    inventory.UseItem(action.itemData, agent);
                    hasUsedItem = true;
                    break;

                case ActionType.MoveToLocation:
                    movementHandler.MoveTo(action.location);
                    movingToLocation = true;
                    break;
                case ActionType.CollectItem:
                    inventory.AddItem(action.itemData);
                    action.item.transform.position += Vector3.up;
                    Destroy(action.item.gameObject);
                    hasCollectedItem = true;
                    break;
                case ActionType.Blueprint:
                    Debug.Log("Now i should make " + action.actionName);
                    break;
                default:
                    Debug.Log(
                        "I don't know how to do that : "
                            + action.actionType
                            + " : "
                            + action.actionName
                    );
                    Debug.Log("Sub plan keys");
                    foreach (Stat key in action.subPlanLists.Keys)
                    {
                        Debug.Log(key.statType);
                    }
                    break;
            }
        }

        private void Update()
        {
            if (currentAction == null)
                return;
            if (!planHandler.executingCurrentPlan)
            {
                statHandler.UpdateGoals();
                planHandler.GenerateCompletePlan(statHandler.currentGoals[0]);
                //GetActionServer();
                return;
            }
            switch (currentAction.actionStatus)
            {
                case ActionStatus.WaitingToExecute:
                    ExecuteNextAction();
                    break;
                case ActionStatus.Executing:
                    if (movingToLocation && movementHandler.HasArrivedAtLocation())
                    {
                        currentAction.actionStatus = ActionStatus.Complete;
                    }
                    if (hasCollectedItem == true)
                    {
                        hasCollectedItem = false;
                        currentAction.actionStatus = ActionStatus.Complete;
                    }
                    if (hasUsedItem)
                    {
                        hasUsedItem = false;
                        currentAction.actionStatus = ActionStatus.Complete;
                    }
                    break;
                case ActionStatus.Complete:

                    if (actionServer.IsLastAction())
                    {
                        isExecutingPlan = false;
                        planHandler.executingCurrentPlan = false;

                        Debug.Log("I have finished my plan");
                    }
                    else
                    {
                        ServeNextAction();
                    }

                    break;
            }
        }
    }
}
