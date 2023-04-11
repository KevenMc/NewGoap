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

        public Agent agent;
        public BlueprintHandler blueprintHandler;
        public InventoryHandler inventory;
        public Action masterAction;
        public Action currentAction;
        public ReverseIterate<Action> actionServer = new ReverseIterate<Action>();
        private List<Action> actionsToPerform = new List<Action>();
        public bool movingToLocation = false;
        public bool hasCollectedItem = false;
        public bool hasUsedItem = false;
        public bool isExecutingPlan = false;
        public bool hasCompletedBlueprint = false;
        public Item equippedItem;
        public bool stop = false;

        public void Init()
        {
            RegisterActionPlanner();
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            UnregisterActionPlanner();
        }

        public void RegisterActionPlanner()
        {
            ActionManager.instance.RegisterSubscriber(this);
            MovementManager.instance.RegisterSubscriber(this);
        }

        public void UnregisterActionPlanner()
        {
            ActionManager.instance.UnregisterSubscriber(this);
            MovementManager.instance.UnregisterSubscriber(this);
        }

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

        public void MoveTo(Vector3 location)
        {
            target = location;
            navMeshAgent.SetDestination(target);
        }

        public bool HasArrivedAtLocation()
        {
            float distance = Vector3.Distance(transform.position, target);
            // Debug.Log(distance);
            if (distance < agent.distanceToArrive)
            {
                MovementManager.instance.UnregisterSubscriber(this);

                navMeshAgent.SetDestination(transform.position);
                currentAction.parentAction.canComplete = true;
                return true;
            }
            return false;
        }

        public void ResetExecution()
        {
            movingToLocation = false;
        }

        private IEnumerator PauseForOneSecond()
        {
            // Pause for one second
            yield return new WaitForSeconds(1f);
        }

        public void ExecuteAction()
        {
            currentAction = actionsToPerform.Last();
            actionsToPerform.Remove(currentAction);
            Debug.Log(
                "Execute an action <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<"
                    + currentAction.actionName
            );
            if (!currentAction.isOwnMaster || currentAction.parentAction.CanComplete())
            {
                actionsToPerform.Add(currentAction.parentAction);
            }

            switch (currentAction.actionType)
            {
                // case ActionType.Use_Item:
                //     inventory.UseItem(action.itemData, agent);
                //     hasUsedItem = true;
                //     break;

                case ActionType.Move_To_Location:
                    MoveTo(currentAction.location);
                    ActionManager.instance.RegisterSubscriber(this);

                    movingToLocation = true;
                    break;

                // case ActionType.Use_Item:
                case ActionType.Equip_From_Inventory:
                case ActionType.UnEquip_To_Inventory:
                case ActionType.Make_Blueprint_From_Inventory:
                case ActionType.Collect_And_Equip:
                    agent.animator.SetBool(currentAction.actionType.ToString(), true);
                    break;

                case ActionType.Blueprint_Require_Item:
                    ExecuteAction();
                    break;

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

        // public void Use_Item()
        // {
        //     inventory.UseItem(equippedItem.itemData, agent);
        //     Destroy(equippedItem.gameObject);
        //     agent.animator.SetBool(ActionType.Use_Item.ToString(), false);
        // }

        public void Make_Blueprint_From_Inventory()
        {
            blueprintHandler.CompleteBlueprintNoStation(currentAction.blueprint);
            agent.animator.SetBool(ActionType.Make_Blueprint_From_Inventory.ToString(), false);
            currentAction.parentAction.canComplete = true;
        }

        public void Collect_And_Equip()
        {
            equippedItem = currentAction.item;
            currentAction.item.transform.SetParent(agent.equipLocation.transform);
            currentAction.item.transform.localPosition = new Vector3();
            agent.animator.SetBool(ActionType.Collect_And_Equip.ToString(), false);
            currentAction.parentAction.canComplete = true;
        }

        public void UnEquip_To_Inventory()
        {
            agent.inventoryHandler.AddItem(currentAction.item.itemData);
            Destroy(currentAction.item.gameObject);
            agent.animator.SetBool(ActionType.UnEquip_To_Inventory.ToString(), false);
            currentAction.parentAction.canComplete = true;
        }

        public void Equip_From_Inventory() { }
    }
}
