using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;

namespace GOAP
{
    public class Action
    {
        public string name;
        public ItemSO item;
        public List<StatEffect> statEffects = new List<StatEffect>();
        public List<Plan> subPlans = new List<Plan>();

        public Action(ItemSO item)
        {
            this.item = item;
            this.name = "Use " + item.itemName;
        }

        public void AddSubPlan(Plan subPlan)
        {
            subPlans.Add(subPlan);
        }

        public void AddStatEffect(StatType statType, int value)
        {
            statEffects.Add(new StatEffect(statType, value));
        }

        public bool IsDoable(Agent agent)
        {
            return true;
        }

        public void ApplyEffects(Agent agent)
        {
            foreach (var statEffect in statEffects)
            {
                agent.statHandler.ModifyStat(statEffect.statType, statEffect.value);
            }
        }
    }

    public class Plan
    {
        public List<Action> actions = new List<Action>();
        public Stat goal;

        public Plan(Stat goal)
        {
            this.goal = goal;
        }

        public void AddAction(Action action)
        {
            actions.Add(action);
        }

        public bool IsComplete(Agent agent)
        {
            // foreach (var statEffect in goal.statEffects)
            // {
            //     if (agent.GetStat(statEffect.statType) < statEffect.value)
            //     {
            //         return false;
            //     }
            // }
            return true;
        }
    }
}
