using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class MovementManager : AbstractManager<ActionHandler>
    {
        public static MovementManager instance;

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

        protected override void PerformTask(ActionHandler subscriber) { }
    }
}
