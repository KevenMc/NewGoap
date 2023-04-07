namespace GOAP
{
    public enum StatType
    {
        Item = 0,
        Station = 1,
        Hunger = 2,
        Thirst = 3,
        Energy = 4
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
