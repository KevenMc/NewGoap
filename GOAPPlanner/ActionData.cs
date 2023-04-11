namespace GOAP
{
    public enum ActionType
    {
        Use_Item = 0,

        //  Collect_Item_To_Inventory,
        Collect_And_Equip,
        UnEquip_To_Inventory,
        Equip_From_Inventory,
        Move_To_Location,
        Make_Blueprint_From_Inventory,
        Blueprint_Require_Item
    }

    public enum ActionStatus
    {
        WaitingToExecute = 0,
        Executing = 1,
        Complete = 2
    }
}
