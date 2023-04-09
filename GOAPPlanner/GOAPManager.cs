using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class GOAPManager : MonoBehaviour
    {
        public static GOAPManager instance;

        private List<ActionPlanner> actionPlanners = new List<ActionPlanner>();

        private IEnumerator actionPlanningCoroutine;

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

        private void Start()
        {
            actionPlanningCoroutine = ActionPlanningCoroutine();
            StartCoroutine(actionPlanningCoroutine);
        }

        private IEnumerator ActionPlanningCoroutine()
        {
            while (true)
            {
                foreach (var planner in actionPlanners)
                {
                    planner.PlanAction();
                }
                yield return null;
            }
        }

        public static void RegisterActionPlanner(ActionPlanner planner)
        {
            instance.actionPlanners.Add(planner);
        }

        public static void UnregisterActionPlanner(ActionPlanner planner)
        {
            instance.actionPlanners.Remove(planner);
        }
    }
}
