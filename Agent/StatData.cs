namespace GOAP
{
    public enum StatType
    {
        Item = 0,
        Station = 1,
        Blueprint = 2,
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
