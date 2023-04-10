using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class ActionHandler : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent navMeshAgent;
        private Vector3 target;

        public void Init() { }

        public void MoveTo(Vector3 location)
        {
            target = location;
            navMeshAgent.SetDestination(target);
        }

        public bool HasArrivedAtLocation()
        {
            float distance = Vector3.Distance(transform.position, target);

            if (distance < agent.distanceToArrive)
            {
                navMeshAgent.SetDestination(transform.position);
                return true;
            }
            return false;
        }

        public Agent agent;
        public BlueprintHandler blueprintHandler;
        public Inventory inventory;
        public Action masterAction;
        public Action currentAction;
        public ReverseIterate<Action> actionServer = new ReverseIterate<Action>();
        private List<Action> actionsToPerform = new List<Action>();
        public bool movingToLocation = false;
        public bool hasCollectedItem = false;
        public bool hasUsedItem = false;
        public bool isExecutingPlan = false;
        public bool hasCompletedBlueprint = false;
        public bool stop = false;

        // public void GetActionServer(Plan plan)
        // {
        //     if (!planHandler.executingCurrentPlan)
        //     {
        //         planHandler.executingCurrentPlan = true;
        //         planHandler.SetActionList();
        //         actionServer = new ReverseIterate<Action>(planHandler.currentActionList);
        //         ServeNextAction();
        //         actionServer.IsLastAction();
        //     }
        // }

        public void SetActionList()
        {
            actionsToPerform.Clear();
            RecursiveSetActions(masterAction);
        }

        private void RecursiveSetActions(Action action)
        {
            if (action.CanComplete())
            {
                actionsToPerform.Add(action);
            }
            else
            {
                foreach (Action childAction in action.childActions)
                {
                    RecursiveSetActions(childAction);
                }
                foreach (Action subAction in action.subActions)
                {
                    RecursiveSetActions(subAction);
                }
            }
        }

        private void ServeNextAction()
        {
            movingToLocation = false;
            hasCollectedItem = false;
            masterAction = actionServer.Next();
        }

        public void ExecuteNextAction()
        {
            ExecuteAction();
        }

        private void ExecuteAction()
        {
            currentAction = actionsToPerform.Last();
            actionsToPerform.Remove(currentAction);

            switch (currentAction.actionType)
            {
                // case ActionType.Use_Item:
                //     inventory.UseItem(action.itemData, agent);
                //     hasUsedItem = true;
                //     break;

                case ActionType.Move_To_Location:
                    MoveTo(currentAction.location);
                    movingToLocation = true;
                    break;
                // case ActionType.Collect_Item:
                //     inventory.AddItem(action.itemData);
                //     action.item.transform.position += Vector3.up;
                //     Destroy(action.item.gameObject);
                //     hasCollectedItem = true;
                //     break;
                // case ActionType.Blueprint_Make:
                //     blueprintHandler.CompleteBlueprintNoStation(action.blueprint);
                //     hasCompletedBlueprint = true;
                //     break;
                default:
                    Debug.Log(
                        "I don't know how to do that : "
                            + currentAction.actionType
                            + " : "
                            + currentAction.actionName
                    );

                    break;
            }
        }

        private void Update()
        {
            if (masterAction == null)
            {
                return;
            }
            else
            {
                Debug.Log("888888888888888888888888888888888888888888888888888888888888888888");
                Debug.Log("There is a master action to complete! " + masterAction.actionName);
            }
            // if (!planHandler.executingCurrentPlan)
            // {
            //     statHandler.UpdateGoals();
            //     planHandler.GenerateCompletePlan(statHandler.currentGoals[0]);
            //     //GetActionServer();
            //     return;
            // }
            switch (masterAction.actionStatus)
            {
                case ActionStatus.WaitingToExecute:
                    ExecuteNextAction();
                    break;
                case ActionStatus.Executing:
                    if (movingToLocation && HasArrivedAtLocation())
                    {
                        masterAction.parentAction.canComplete = true;
                        masterAction.actionStatus = ActionStatus.Complete;
                    }
                    if (hasCollectedItem == true)
                    {
                        hasCollectedItem = false;
                        masterAction.actionStatus = ActionStatus.Complete;
                    }
                    if (hasUsedItem)
                    {
                        hasUsedItem = false;
                        masterAction.parentAction.canComplete = true;

                        masterAction.actionStatus = ActionStatus.Complete;
                    }
                    if (hasCompletedBlueprint)
                    {
                        hasCompletedBlueprint = false;
                        masterAction.parentAction.canComplete = true;
                        masterAction.actionStatus = ActionStatus.Complete;
                    }
                    break;
                case ActionStatus.Complete:

                    if (actionServer.IsLastAction())
                    {
                        isExecutingPlan = false;

                        Debug.Log("I have finished my plan");
                        stop = true;
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
