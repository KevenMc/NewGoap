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
        Move_To_Agent,
        Require_Move_To_Location,
        Interact_With_Station,
        Equip_From_Station,
        Equip_From_Storage,
        UnEquip_To_Storage,
        Make_Blueprint_From_Inventory,
        Make_Blueprint_At_Station,
        Blueprint_Require_Item,
        Delegate_Action,
        Receive_Delegate_Action,
        Return_Delegate_Action
    }

    public enum ActionStatus
    {
        WaitingToExecute = 0,
        Executing = 1,
        Complete = 2
    }
}
