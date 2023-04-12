using System;
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
        }

        private void OnEnable()
        {
            RegisterStatHandler();
        }

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

        public void UpdateGoals()
        {
            currentGoals = stats
                .Where(stat => stat.current >= stat.trigger)
                .OrderByDescending(stat => stat.priority)
                .ToList();
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
