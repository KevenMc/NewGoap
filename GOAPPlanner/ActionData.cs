namespace GOAP
{
    public enum ActionType
    {
        Use_Item = 0,
        Collect_Item = 1,
        Move_To_Location = 2,
        Blueprint_Make = 3,
        Blueprint_Require_Item = 4
    }

    public enum ActionStatus
    {
        WaitingToExecute = 0,
        Executing = 1,
        Complete = 2
    }
}
