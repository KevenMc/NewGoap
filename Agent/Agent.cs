using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    [RequireComponent(typeof(StatHandler))]
    // [RequireComponent(typeof(ActionPlanner))]
    [RequireComponent(typeof(InventoryHandler))]
    [RequireComponent(typeof(KnowledgeHandler))]
    [RequireComponent(typeof(BlueprintHandler))]
    [RequireComponent(typeof(MovementHandler))]
    public class Agent : MonoBehaviour
    {
        public StatHandler statHandler;

        // public ActionPlanner actionPlanner;
        public InventoryHandler inventoryHandler;
        public KnowledgeHandler knowledgeHandler;
        public BlueprintHandler blueprintHandler;
        public ActionHandler actionHandler;
        public Animator animator;
        public string currentGoal;
        public float distanceToArrive = 1f;
        public Boolean requiresNewAction = true;
        public GameObject equipLocation;

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

            Animator animator = GetComponent<Animator>();

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
            Debug.Log("Set master action");
            actionHandler.masterAction = action;
            actionHandler.SetActionList();
            actionHandler.ExecuteAction();
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
