using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using UnityEngine;

namespace GOAP
{
    public class StatHandler : MonoBehaviour
    {
        public Agent agent;

        public List<StatPassport> statPassports = new List<StatPassport>();
        public List<Stat> stats = new List<Stat>();

        public Dictionary<StatType, Stat> statsByStatType = new Dictionary<StatType, Stat>();
        public List<Stat> currentGoals = new List<Stat>();

        public class StatHeader
        {
            public StatType statType;
            public int priority;
            public Boolean isUrgent;

            public StatHeader(Stat stat)
            {
                this.statType = stat.statType;
                this.priority = stat.priority;
                this.isUrgent = stat.isUrgent;
            }
        }

        public void Init()
        {
            CompileStatPassports();
            UpdateGoals();
            StatManager.instance.RegisterSubscriber(this);
            RegisterStatHandler();
        }

        // private void OnEnable()
        // {
        //     RegisterStatHandler();
        // }

        private void OnDisable()
        {
            UnregisterStatHandler();
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
            }
        }

        public void CheckAndUpdateGoals()
        {
            foreach (Stat stat in stats)
            {
                if (stat.current >= stat.trigger && !currentGoals.Contains(stat))
                {
                    // Add the stat to the current goals list
                    currentGoals.Add(stat);
                    // Sort the current goals list based on priority
                    currentGoals = currentGoals.OrderByDescending(s => s.priority).ToList();

                    agent.RegisterActionPlanner();
                }
            }
        }

        public void UpdateGoals()
        {
            currentGoals.Clear();
            currentGoals.AddRange(
                stats
                    .Where(stat => stat.current >= stat.trigger)
                    .OrderByDescending(stat => stat.priority)
                    .ToList()
            );
        }

        public void RegisterStatHandler()
        {
            StatManager.instance.RegisterSubscriber(this);
        }

        public void UnregisterStatHandler()
        {
            StatManager.instance.UnregisterSubscriber(this);
        }
    }
}
