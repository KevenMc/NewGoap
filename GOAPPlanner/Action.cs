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
        public ItemSO itemData;
        public Item item;
        public List<StatEffect> statEffects = new List<StatEffect>();
        public List<Plan> subPlans = new List<Plan>();

        public Action(ItemSO itemData) //CCreate action from inventory
        {
            this.itemData = itemData;
            this.actionName = "Use " + itemData.itemName;
        }

        public Action(ItemSO itemData, Item item)
        {
            this.itemData = itemData;
            this.item = item;
            this.actionName = "Collect " + itemData.itemName;
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
