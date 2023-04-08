namespace GOAP
{
    public enum ActionType
    {
        UseItem = 0,
        CollectItem = 1,
        MoveToLocation = 2,
        Blueprint = 3,
        BlueprintItem = 4
    }

    public enum ActionStatus
    {
        WaitingToExecute = 0,
        Executing = 1,
        Complete = 2
    }
}
