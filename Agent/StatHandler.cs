using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class StatHandler : MonoBehaviour
    {
        public List<Stat> stats = new List<Stat>();
        public List<Stat> currentGoals = new List<Stat>();

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            // Update the current value of each stat
            foreach (var stat in stats)
            {
                stat.current += stat.increment * deltaTime;
            }
            UpdateGoals();
            // Update current goals list based on stat priorities
        }

        public void UpdateGoals()
        {
            currentGoals = stats
                .Where(stat => stat.current >= stat.trigger)
                .OrderByDescending(stat => stat.priority)
                //.Select(stat => stat)
                .ToList();
            // Debug.Log(currentGoals[0].statType);
        }

        public void ModifyStat(StatType statType, float value)
        {
            Stat stat = stats.Find(x => x.statType == statType);
            if (stat != null)
            {
                stat.current += value;
            }
            else
            {
                Debug.LogWarning(
                    "StatHandler: Could not find stat with type " + statType.ToString()
                );
            }
        }
    }
}
