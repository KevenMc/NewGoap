using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class ActionPlannerManager : MonoBehaviour
    {
        public static ActionPlannerManager instance;
        private List<ActionPlanner> actionPlanners = new List<ActionPlanner>();
        private IEnumerator actionPlannerCoroutine;
        public float refreshRate = 0.1f;

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
            actionPlannerCoroutine = ActionPlannerCoroutine();
            StartCoroutine(actionPlannerCoroutine);
        }

        private IEnumerator ActionPlannerCoroutine()
        {
            while (true)
            {
                Debug.Log("Action Planner Manager");
                yield return new WaitForSeconds(refreshRate);

                foreach (var planner in actionPlanners)
                {
                    planner.PlanAction();
                }
                yield return null;
            }
        }

        public static void RegisterActionPlanner(ActionPlanner planner)
        {
            if (instance.actionPlanners.Contains(planner))
                return;
            instance.actionPlanners.Add(planner);
        }

        public static void UnregisterActionPlanner(ActionPlanner planner)
        {
            instance.actionPlanners.Remove(planner);
        }
    }
}
