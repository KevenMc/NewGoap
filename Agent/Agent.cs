using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    [RequireComponent(typeof(RelationshipHandler))]
    // [RequireComponent(typeof(NavMeshAgent))]
    public class Agent : MonoBehaviour
    {
        public string agentName;
        public StatHandler statHandler;

        // public ActionPlanner actionPlanner;
        public InventoryHandler inventoryHandler;
        public Inventory inventory;
        public KnowledgeHandler knowledgeHandler;
        public RelationshipHandler relationshipHandler;
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
            inventory = GetComponent<Inventory>();
            knowledgeHandler = GetComponent<KnowledgeHandler>();
            relationshipHandler = GetComponent<RelationshipHandler>();
            blueprintHandler = GetComponent<BlueprintHandler>();
            actionHandler = GetComponent<ActionHandler>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            statHandler.Init();
            // actionPlanner.Init();
            actionHandler.Init();
            inventoryHandler.Init();
            knowledgeHandler.Init();
            blueprintHandler.Init();
            actionHandler.Init();

            RegisterActionPlanner();
        }

        public void SetMasterAction(Action action)
        {
            Debug.Log("Setting master action");
            UnregisterActionPlanner();
            actionHandler.masterAction = action;
            actionHandler.SetActionList();
            actionHandler.ExecuteAction();
        }

        public void InstantiateEquippedItem(GameObject itemPrefab)
        {
            Debug.Log("Instantiating equipped item " + itemPrefab);
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
