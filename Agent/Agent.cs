using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

namespace GOAP
{
    [RequireComponent(typeof(StatHandler))]
    [RequireComponent(typeof(InventoryHandler))]
    [RequireComponent(typeof(KnowledgeHandler))]
    [RequireComponent(typeof(BlueprintHandler))]
    [RequireComponent(typeof(MovementHandler))]
    [RequireComponent(typeof(ActionHandler))]
    [RequireComponent(typeof(Inventory))]
    // [RequireComponent(typeof(NavMeshAgent))]
    public class Agent : MonoBehaviour
    {
        public StatHandler statHandler;

        // public ActionPlanner actionPlanner;
        public InventoryHandler inventoryHandler;
        public Inventory inventory;
        public KnowledgeHandler knowledgeHandler;
        public BlueprintHandler blueprintHandler;
        public ActionHandler actionHandler;
        public NavMeshAgent navMeshAgent;
        public Animator animator;
        public string currentGoal;
        public float distanceToArrive = 1f;
        public Boolean requiresNewAction = true;
        public GameObject equipLocation;
        public GameObject equippedItem;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            statHandler = GetComponent<StatHandler>();
            // actionPlanner = GetComponent<ActionPlanner>();
            inventoryHandler = GetComponent<InventoryHandler>();
            knowledgeHandler = GetComponent<KnowledgeHandler>();
            blueprintHandler = GetComponent<BlueprintHandler>();
            actionHandler = GetComponent<ActionHandler>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            statHandler.Init();
            // actionPlanner.Init();
            inventoryHandler.Init();
            knowledgeHandler.Init();
            blueprintHandler.Init();
            actionHandler.Init();

            RegisterActionPlanner();
        }

        public void SetMasterAction(Action action)
        {
            actionHandler.masterAction = action;
            actionHandler.SetActionList();
            actionHandler.ExecuteAction();
        }

        public void InstantiateEquippedItem(GameObject itemPrefab)
        {
            equippedItem = Instantiate(itemPrefab);
            equippedItem.transform.SetParent(equipLocation.transform);
            equippedItem.transform.localPosition = new Vector3();
        }

        private void OnDisable()
        {
            UnregisterActionPlanner();
        }

        public void RegisterActionPlanner()
        {
            ActionPlannerManager.instance.RegisterSubscriber(this);
        }

        public void UnregisterActionPlanner()
        {
            ActionPlannerManager.instance.UnregisterSubscriber(this);
        }
    }
}
