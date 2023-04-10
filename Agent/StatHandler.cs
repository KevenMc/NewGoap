using System.Net.Mail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class StatHandler : MonoBehaviour
    {
        public List<StatPassport> statPassports = new List<StatPassport>();
        public List<Stat> stats = new List<Stat>();

        public Dictionary<StatType, Stat> statsByStatType = new Dictionary<StatType, Stat>();
        public List<Stat> currentGoals = new List<Stat>();
        public Stat currentGoal;

        public void Init()
        {
            CompileStatPassports();
            UpdateGoals();
            Debug.Log("NOW REGISTER STATMANAGER");
            StatManager.instance.RegisterSubscriber(this);
        }

        private void OnEnable()
        {
            StatManager.instance.RegisterSubscriber(this);
        }

        private void OnDisable()
        {
            StatManager.instance.UnregisterSubscriber(this);
        }

        public void Start() { }

        public void CompileStatPassports()
        {
            foreach (StatPassport statPassport in statPassports)
            {
                foreach (Stat stat in statPassport.stats)
                {
                    if (!stats.Contains(stat))
                    {
                        stats.Add(new Stat(stat));
                    }
                }
            }
            foreach (Stat stat in stats)
            {
                statsByStatType[stat.statType] = stat;
                Debug.Log("Adding " + stat.statType + " to stat dict");
            }
        }

        public void UpdateGoals()
        {
            currentGoals = stats
                .Where(stat => stat.current >= stat.trigger)
                .OrderByDescending(stat => stat.priority)
                .ToList();
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
