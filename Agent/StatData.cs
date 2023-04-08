namespace GOAP
{
    public enum StatType
    {
        HaveItem = 0,
        IsAtLocation = 1,
        MoveTo = 2,
        Station = 3,
        Blueprint = 4,
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
