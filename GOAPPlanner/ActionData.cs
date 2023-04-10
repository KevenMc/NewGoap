namespace GOAP
{
    public enum ActionType
    {
        Use_Item = 0,
        Collect_Item_To_Inventory = 1,
        Collect_Item_To_Equip = 2,
        Equip_From_Inventory = 3,
        Move_To_Location = 4,
        Blueprint_Make = 5,
        Blueprint_Require_Item = 6
    }

    public enum ActionStatus
    {
        WaitingToExecute = 0,
        Executing = 1,
        Complete = 2
    }
}
