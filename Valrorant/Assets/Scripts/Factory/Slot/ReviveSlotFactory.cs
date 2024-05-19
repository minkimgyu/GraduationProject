using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

[Serializable]
public class ReviveSlotData : SlotData
{
    public ReviveSlotData(SlotPlant.Name name, int cost, string info, Database.IconName iconName) 
        : base(name, cost, info, iconName)
    {
    }
}

public class ReviveSlotFactory : SlotFactory<ReviveSlotData>
{
    public override ItemSlot Create(Func<ShopBlackboard> ReturnBlackboard)
    {
        ItemSlot slot = Object.Instantiate(_prefab).GetComponent<ItemSlot>();
        slot.Initialize(_data, ReturnBlackboard);
        return slot;
    }
}
