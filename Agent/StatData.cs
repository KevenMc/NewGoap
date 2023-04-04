namespace GOAP
{
    public enum StatType
    {
        Hunger = 0,
        Thirst = 1,
        Energy = 2
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
