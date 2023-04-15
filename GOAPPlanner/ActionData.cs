namespace GOAP
{
    public enum ActionType
    {
        Use_Item,

        Collect_And_Equip,
        UnEquip_To_Inventory,
        Equip_From_Inventory,
        Require_Item_In_Inventory,
        Move_To_Location,
        Interact_With_Station,
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
