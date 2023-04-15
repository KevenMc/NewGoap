using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class Goal
    {
        public GoalType goalType;
        public StatType statType;

        public Goal(Stat stat)
        {
            this.goalType = GoalType.Satisfies_Stat;
            this.statType = stat.statType;
        }
    }
}
