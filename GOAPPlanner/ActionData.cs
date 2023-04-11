namespace GOAP
{
    public enum ActionType
    {
        Use_Item = 0,

        //  Collect_Item_To_Inventory,
        Collect_To_Equip,
        UnEquip_To_Inventory,
        Inventory_To_Equip,
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
