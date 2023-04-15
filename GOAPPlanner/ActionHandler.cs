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
        public InventoryHandler inventoryHandler;
        public Action masterAction;
        public Action currentAction;
        public ReverseIterate<Action> actionServer = new ReverseIterate<Action>();
        private List<Action> actionsToPerform = new List<Action>();

        public Item equippedItem;

        public void Init()
        {
            RegisterActionHandler();
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            UnregisterActionHandler();
        }

        public void RegisterActionHandler()
        {
            ActionManager.instance.RegisterSubscriber(this);
            MovementManager.instance.RegisterSubscriber(this);
        }

        public void UnregisterActionHandler()
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

        public void ExecuteAction()
        {
            currentAction = actionsToPerform.Last();
            if (currentAction.parentAction == null)
            {
                Debug.Log("There is no more action to execute");
                agent.requiresNewAction = true;
                return;
            }
            actionsToPerform.Remove(currentAction);
            Debug.Log(
                "Execute an action <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<"
                    + currentAction.actionName
                    + " : "
                    + currentAction?.goal?.statType
            );
            if (!currentAction.isOwnMaster || currentAction.parentAction.CanComplete())
            {
                Debug.Log("Adding : " + currentAction.parentAction);
                actionsToPerform.Add(currentAction.parentAction);
            }

            switch (currentAction.actionType)
            {
                case ActionType.Move_To_Location:
                    MoveTo(currentAction.location);
                    ActionManager.instance.RegisterSubscriber(this);

                    break;

                case ActionType.Equip_From_Inventory:
                case ActionType.UnEquip_To_Inventory:
                case ActionType.Make_Blueprint_From_Inventory:
                case ActionType.Collect_And_Equip:
                case ActionType.Interact_With_Station:
                    agent.animator.SetBool(currentAction.actionType.ToString(), true);
                    break;

                case ActionType.Use_Item:
                    Debug.Log(currentAction.itemData.itemName);
                    agent.animator.Play(currentAction.itemData.useItemAnimation.name);
                    agent.animator.SetBool(currentAction.actionType.ToString(), true);

                    break;

                case ActionType.Blueprint_Require_Item:
                case ActionType.Require_Item_In_Inventory:
                    currentAction.parentAction.canComplete = true;
                    Debug.Log("Now executing a new action, this action is empty");
                    Debug.Log(actionsToPerform.Count);
                    ExecuteAction();
                    Debug.Log("Executed");
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

        public void Use_Item()
        {
            inventoryHandler.UseItem(currentAction.itemData, agent);
            Destroy(agent.equippedItem.gameObject);
            agent.equippedItem = null;
            agent.animator.SetBool(ActionType.Use_Item.ToString(), false);

            ExecuteAction();
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
            CompleteBlueprintNoStation();
            agent.animator.SetBool(ActionType.Make_Blueprint_From_Inventory.ToString(), false);
            currentAction.parentAction.canComplete = true;
        }

        public void CompleteBlueprintNoStation()
        {
            foreach (Blueprint.ItemRequirement item in currentAction.blueprint.requiredItems)
            {
                if (item.destroyOnCraft)
                {
                    inventoryHandler.RemoveItem(item.itemData);
                }
            }
            inventoryHandler.AddItem(currentAction.blueprint.craftedItem);
        }

        public void Collect_And_Equip()
        {
            equippedItem = currentAction.item;
            currentAction.item.transform.SetParent(agent.equipLocation.transform);
            currentAction.item.transform.localPosition = new Vector3();
            agent.equippedItem = equippedItem.gameObject;

            agent.animator.SetBool(ActionType.Collect_And_Equip.ToString(), false);
            currentAction.parentAction.canComplete = true;
        }

        public void UnEquip_To_Inventory()
        {
            inventoryHandler.AddItem(currentAction.item.itemData);
            Destroy(currentAction.item.gameObject);
            agent.animator.SetBool(ActionType.UnEquip_To_Inventory.ToString(), false);
            currentAction.parentAction.canComplete = true;
        }

        public void Equip_From_Inventory()
        {
            agent.InstantiateEquippedItem(currentAction.itemData.itemPrefab);
            agent.animator.SetBool(ActionType.Equip_From_Inventory.ToString(), false);
        }
    }
}
