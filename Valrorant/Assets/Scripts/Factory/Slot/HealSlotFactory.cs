using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

[Serializable]
public class HealSlotData : SlotData
{
    public float hpRatio;

    public HealSlotData(SlotPlant.Name name, int cost, string info, Database.IconName iconName, float hpRatio) 
        : base(name, cost, info, iconName)
    {
        this.hpRatio = hpRatio;
    }
}

public class HealSlotFactory : SlotFactory<HealSlotData>
{
    public override ItemSlot Create(Func<ShopBlackboard> ReturnBlackboard)
    {
        ItemSlot slot = Object.Instantiate(_prefab).GetComponent<ItemSlot>();
        slot.Initialize(_data, ReturnBlackboard);
        return slot;
    }
}
