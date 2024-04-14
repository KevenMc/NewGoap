using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP
{
    public class StatManager : AbstractManager<StatHandler>
    {
        public static StatManager instance;

        public Dictionary<StatHandler, List<StatEffect>> statsToModify =
            new Dictionary<StatHandler, List<StatEffect>>();

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
            if (statsToModify.ContainsKey(subscriber))
            {
                foreach (StatEffect statEffect in statsToModify[subscriber])
                {
                    ModifyStat(subscriber, statEffect);
                }
                statsToModify.Remove(subscriber);
            }
            subscriber.CheckAndUpdateGoals();
        }

        public void ModifyStat(StatHandler subscriber, StatEffect statEffect)
        {
            Debug.Log("Modify stat");
            Stat stat = subscriber.stats.Find(x => x.statType == statEffect.statType);
            if (stat != null)
            {
                Debug.Log("1");
                stat.current += statEffect.value;
            }
            else
            {
                Debug.LogWarning(
                    "StatHandler: Could not find stat with type " + statEffect.statType.ToString()
                );
            }
        }
    }
}
