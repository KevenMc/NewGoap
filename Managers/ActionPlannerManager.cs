using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class ActionPlannerManager : AbstractManager<ActionPlanner>
    {
        public static ActionPlannerManager instance;

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

        protected override void PerformTask(ActionPlanner subscriber)
        {
            subscriber.PlanAction();
        }
    }
}
