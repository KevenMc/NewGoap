using UnityEngine;

namespace GOAP
{
    public enum StatType
    {
        [HideInInspector]
        Null = 0,

        [HideInInspector]
        HaveItem = 1,

        [HideInInspector]
        IsAtLocation = 2,

        [HideInInspector]
        MoveTo = 3,

        [HideInInspector]
        Station = 4,
        Blueprint = 5,
        Hunger = 100,
        Thirst = 101,
        Energy = 102
    }

    public class StatEffect
    {
        public StatType statType;
        public float value;

        public StatEffect(StatType statType, float value)
        {
            this.statType = statType;
            this.value = value;
        }
    }
}
