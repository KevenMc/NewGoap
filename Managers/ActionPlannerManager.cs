using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class ActionPlannerManager : AbstractManager<Agent>
    {
        public static ActionPlannerManager instance;

        public ActionPlanner actionPlanner;

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

        protected override void PerformTask(Agent subscriber)
        {
            actionPlanner.PlanAction(subscriber);
            subscriber.UnregisterActionPlanner();
        }
    }
}
