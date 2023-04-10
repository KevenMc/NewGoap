using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class StatManager : AbstractManager<StatHandler>
    {
        public static StatManager instance;

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

        protected override void PerformTask(StatHandler subscriber)
        {
            foreach (Stat stat in subscriber.stats)
            {
                stat.current += stat.increment * refreshRate;
            }
        }
    }
}
