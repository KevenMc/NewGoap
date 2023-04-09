using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    [RequireComponent(typeof(StatHandler))]
    [RequireComponent(typeof(ActionPlanner))]
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(KnowledgeHandler))]
    [RequireComponent(typeof(BlueprintHandler))]
    [RequireComponent(typeof(MovementHandler))]
    public class Agent : MonoBehaviour
    {
        public StatHandler statHandler;
        public ActionPlanner actionPlanner;
        public Inventory inventory;
        public KnowledgeHandler knowledgeHandler;
        public BlueprintHandler blueprintHandler;
        public MovementHandler movementHandler;
        public string currentGoal;
        public float distanceToArrive = 1f;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            statHandler = GetComponent<StatHandler>();
            actionPlanner = GetComponent<ActionPlanner>();
            inventory = GetComponent<Inventory>();
            knowledgeHandler = GetComponent<KnowledgeHandler>();
            blueprintHandler = GetComponent<BlueprintHandler>();
            movementHandler = GetComponent<MovementHandler>();

            statHandler.Init();
            actionPlanner.Init();
            inventory.Init();
            knowledgeHandler.Init();
            blueprintHandler.Init();
            movementHandler.Init();
        }
    }
}
