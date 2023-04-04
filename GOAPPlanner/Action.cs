using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;

namespace GOAP
{
    public class Action
    {
        public string actionName;
        public int actionCost = 0;
        public Stat goal;
        public ItemSO item;
        public List<StatEffect> statEffects = new List<StatEffect>();
        public List<Plan> subPlans = new List<Plan>();

        public Action(ItemSO item)
        {
            this.item = item;
            this.actionName = "Use " + item.itemName;
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
}
