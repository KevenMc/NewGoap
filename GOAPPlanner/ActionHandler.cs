using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP
{
    public class ActionHandler : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent navMeshAgent;
        public Vector3 target;
        public Agent agent;
        public BlueprintHandler blueprintHandler;
        public InventoryHandler inventoryHandler;
        public Action masterAction;
        public Action currentAction;
        public bool canPerformAction = true;
        public string performAction;
        public float distance;
        public ReverseIterate<Action> actionServer = new ReverseIterate<Action>();
        private List<Action> actionsToPerform = new List<Action>();

        public Item equippedItem;

        void Update()
        {
            // Calculate the direction from the character to the target
            Vector3 direction = (target - transform.position).normalized;

            // Calculate the angle between the forward direction of the character and the direction to the target
            float angle = Vector3.Angle(transform.forward, direction);

            // Determine if the target is to the left or right of the character
            float sign = Mathf.Sign(
                Vector3.Dot(Vector3.up, Vector3.Cross(transform.forward, direction))
            );

            // Calculate the horizontal and vertical values based on the angle
            float horizontal = sign * Mathf.Sin(angle * Mathf.Deg2Rad);
            float vertical = Mathf.Cos(angle * Mathf.Deg2Rad);

            // Set the animator parameters
            agent.animator.SetFloat("Horizontal", horizontal);
            agent.animator.SetFloat("Vertical", vertical);
        }

        public void Init()
        {
            RegisterActionHandler();
        }

        // private void OnEnable()
        // {
        //     Init();
        // }

        private void OnDisable()
        {
            UnregisterActionHandler();
        }

        public void SetAnimatorState(int state)
        {
            agent.animator.SetInteger("State", state);
            agent.animator.SetTrigger("OnState");
        }

        public void RegisterActionHandler()
        {
            ActionManager.instance.RegisterSubscriber(this);
            MovementManager.instance.RegisterSubscriber(this);
        }

        public void UnregisterActionHandler()
        {
            Debug.Log("Unregister from action and movement manager");
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
                if (action.actionType != ActionType.Require_Item_In_Inventory)
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
            SetAnimatorState(1);
            target = location;
            navMeshAgent.SetDestination(target);
            navMeshAgent.isStopped = false;
        }

        public bool HasArrivedAtLocation()
        {
            distance = Vector3.Distance(transform.position, target);
            if (distance < agent.distanceToArrive)
            {
                MovementManager.instance.UnregisterSubscriber(this);
                navMeshAgent.isStopped = true;
                SetAnimatorState(0);

                navMeshAgent.SetDestination(transform.position);
                currentAction.parentAction.canComplete = true;
                return true;
            }
            return false;
        }

        public void ExecuteAction()
        {
            Debug.Log("Number of actions left to perform : " + actionsToPerform.Count);
            Debug.Log("0000000000000000000000000000000000000000000000000000000000");
            foreach (Action act in actionsToPerform)
            {
                Debug.Log(act.actionName);
            }
            Debug.Log("0000000000000000000000000000000000000000000000000000000000");

            if (!canPerformAction)
            {
                Debug.Log("Cannot perform next action");
                return;
            }
            Debug.Log("Execute");
            MoveTo(transform.position);

            agent.animator.SetBool("Use_Item", false);
            agent.animator.SetBool("Collect_And_Equip", false);
            agent.animator.SetBool("Make_Blueprint_At_Station", false);
            agent.animator.SetBool(ActionType.UnEquip_To_Inventory.ToString(), false);
            agent.animator.SetBool(ActionType.Make_Blueprint_From_Inventory.ToString(), false);

            currentAction = actionsToPerform.Last();
            performAction = currentAction.actionName;

            actionsToPerform.Remove(currentAction);
            Debug.Log(
                "Execute an action <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<"
                    + currentAction.actionName
                    + " : "
                    + currentAction?.goal?.statType
                    + " : "
                    + currentAction.CanComplete()
            );
            Debug.Log(currentAction.actionType.ToString());
            if (currentAction.actionType == ActionType.Master_Action)
            {
                SetAnimatorState(0);
                agent.RegisterActionPlanner();
                agent.requiresNewAction = true;
                Debug.Log("This plan has concluded");
                return;
            }

            if (!currentAction.isOwnMaster || currentAction.parentAction.CanComplete())
            {
                actionsToPerform.Add(currentAction.parentAction);
                actionsToPerform.Remove(currentAction);
            }
            if (!currentAction.CanComplete())
            {
                RecursiveSetActions(currentAction);
            }
            SetAnimatorState(0);

            // currentAction.LogAction();
            switch (currentAction.actionType)
            {
                case ActionType.Make_Blueprint_At_Station:
                    agent.animator.SetBool(currentAction.actionType.ToString(), true);
                    break;
                case ActionType.Move_To_Location:
                    Debug.Log("MoveTO");
                    MoveTo(currentAction.location);
                    if (HasArrivedAtLocation())
                    {
                        MoveTo(transform.position);
                        ExecuteAction();
                        break;
                    }
                    ActionManager.instance.RegisterSubscriber(this);
                    break;

                case ActionType.Equip_From_Inventory:
                    // if (agent.equippedItem.GetComponent<Item>().itemData == currentAction.itemData)
                    // {
                    //     // Debug.Log(currentAction.actionName + " : " + currentAction.canComplete);
                    //     // Debug.Log(
                    //     //     currentAction.parentAction.actionName
                    //     //         + " : "
                    //     //         + currentAction.parentAction.canComplete
                    //     // );
                    //     // currentAction.parentAction.canComplete = true;
                    //     ExecuteAction();
                    //     break;
                    // }

                    Equip_From_Inventory();
                    break;
                case ActionType.UnEquip_To_Inventory:
                    Debug.Log("Unequip this item : " + currentAction.itemData.itemName);
                    agent.animator.SetBool(ActionType.UnEquip_To_Inventory.ToString(), true);
                    break;
                case ActionType.Make_Blueprint_From_Inventory:
                    if (currentAction.subActions.Count > 0)
                    {
                        bool canComplete = true;
                        foreach (Action subAct in currentAction.subActions)
                        {
                            if (!subAct.CanComplete())
                            {
                                canComplete = false;
                                RecursiveSetActions(subAct);
                                ExecuteAction();
                                break;
                            }
                            else { }
                        }

                        if (canComplete)
                        {
                            agent.animator.SetBool(
                                ActionType.Make_Blueprint_From_Inventory.ToString(),
                                true
                            );
                        }
                    }

                    if (currentAction.childActions.Count > 0)
                    {
                        // Debug.Log("ChildAction : " + currentAction.childActions[0].actionName);
                        actionsToPerform.Add(currentAction.childActions[0]);
                        currentAction.childActions.RemoveAt(0);
                        ExecuteAction();
                        break;
                    }

                    break;
                case ActionType.Collect_And_Equip:
                case ActionType.Interact_With_Station:

                    agent.animator.SetBool(currentAction.actionType.ToString(), true);
                    break;

                case ActionType.Use_Item:
                    agent.animator.SetBool(currentAction.actionType.ToString(), true);
                    SetAnimatorState(0);
                    break;

                case ActionType.Require_Move_To_Location:

                    if (currentAction.childActions.Count > 0)
                    {
                        actionsToPerform.Add(currentAction.childActions[0]);
                        currentAction.childActions.RemoveAt(0);
                    }
                    ExecuteAction();

                    break;
                case ActionType.Blueprint_Require_Item:
                case ActionType.Require_Item_In_Inventory:
                case ActionType.Delegate_Action:
                case ActionType.Move_To_Agent:
                case ActionType.Equip_From_Station:
                case ActionType.Equip_From_Storage:
                case ActionType.UnEquip_To_Storage:
                case ActionType.Receive_Delegate_Action:
                case ActionType.Return_Delegate_Action:
                    ExecuteAction();
                    break;

                default:
                    Debug.Log(
                        "I don't know how to do that : "
                            + currentAction.actionType
                            + " : "
                            + currentAction.actionName
                    );
                    ExecuteAction();
                    break;
            }

            agent.animator.SetTrigger("Do");
            // if (currentAction.parentAction.isOwnMaster)
            // {
            //     Debug.Log("This action has no parent!");
            //     agent.requiresNewAction = true;
            //     return;
            // }
        }

        public void Use_Item()
        {
            // inventoryHandler.UseItem(currentAction.itemData, agent.inventory, agent);
            if (!StatManager.instance.statsToModify.ContainsKey(agent.statHandler))
            {
                StatManager.instance.statsToModify[agent.statHandler] = new List<StatEffect>();
            }
            foreach (StatEffect statEffect in currentAction.itemData.statEffects)
            {
                StatManager.instance.statsToModify[agent.statHandler].Add(statEffect);
            }
            // foreach (var statEffect in itemData.statEffects)
            // {
            //     useAgent.statHandler.ModifyStat(statEffect.statType, statEffect.value);
            // }

            Destroy(agent.equippedItem.gameObject);
            agent.equippedItem = null;
            agent.animator.SetBool(ActionType.Use_Item.ToString(), false);

            // ExecuteAction();
            // agent.animator.SetBool(ActionType.Make_Blueprint_From_Inventory.ToString(), false);
        }

        public void Use_Station()
        {
            agent.animator.SetBool(ActionType.Interact_With_Station.ToString(), false);

            if (!StatManager.instance.statsToModify.ContainsKey(agent.statHandler))
            {
                StatManager.instance.statsToModify[agent.statHandler] = new List<StatEffect>();
            }
            foreach (StatEffect statEffect in currentAction.stationData.statEffects)
            {
                StatManager.instance.statsToModify[agent.statHandler].Add(statEffect);
            }
        }

        public void Make_Blueprint_From_Inventory()
        {
            agent.animator.SetBool(ActionType.Make_Blueprint_From_Inventory.ToString(), false);

            CompleteBlueprintNoStation();
            currentAction.parentAction.canComplete = true;
            ExecuteAction();
        }

        public void CompleteBlueprintNoStation()
        {
            foreach (Blueprint.ItemRequirement item in currentAction.blueprint.requiredItems)
            {
                if (item.destroyOnCraft)
                {
                    inventoryHandler.RemoveItem(item.itemData, agent.inventory);
                }
            }
            CompleteBlueprint();
        }

        public void CompleteBlueprint()
        {
            agent.InstantiateEquippedItem(currentAction.blueprint.craftedItem.itemPrefab);
            // inventoryHandler.AddItem(currentAction.blueprint.craftedItem, agent.inventory);
        }

        public void Collect_And_Equip()
        {
            equippedItem = currentAction.item;
            agent.knowledgeHandler.itemMemory.RemoveItem(currentAction.item);
            currentAction.item.transform.SetParent(agent.equipLocation.transform);
            currentAction.item.transform.localPosition = new Vector3();
            agent.equippedItem = equippedItem.gameObject;

            agent.animator.SetBool(ActionType.Collect_And_Equip.ToString(), false);
            currentAction.parentAction.canComplete = true;
        }

        public void UnEquip_To_Inventory()
        {
            inventoryHandler.AddItem(currentAction.itemData, agent.inventory);
            Destroy(agent.equippedItem);
            agent.animator.SetBool(ActionType.UnEquip_To_Inventory.ToString(), false);
            currentAction.parentAction.canComplete = true;
            agent.equippedItem = null;
        }

        public void Equip_From_Inventory()
        {
            Debug.Log("Equipping from inventory?");
            Debug.Log(agent.equippedItem);
            if (
                agent.equippedItem != null
                && agent.equippedItem.GetComponent<Item>().itemData == currentAction.itemData
            )
            {
                Debug.Log("Item is already equipped");
                ExecuteAction();
                return;
            }
            Debug.Log("Item is not equipped");
            agent.InstantiateEquippedItem(currentAction.itemData.itemPrefab);
            agent.animator.SetBool(ActionType.Equip_From_Inventory.ToString(), false);
        }
    }
}
