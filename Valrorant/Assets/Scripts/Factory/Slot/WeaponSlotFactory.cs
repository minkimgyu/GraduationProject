using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

[Serializable]
public class SlotData
{
    public SlotPlant.Name name;
    public int cost;
    public string info;
    public Database.IconName iconName;

    public SlotData(SlotPlant.Name name, int cost, string info, Database.IconName iconName)
    {
        this.name = name;
        this.cost = cost;
        this.info = info;
        this.iconName = iconName;
    }
}

public class WeaponSlotData : SlotData
{
    public BaseWeapon.Name weaponNameToSpawn;

    public WeaponSlotData(SlotPlant.Name name, int cost, string info, Database.IconName iconName, BaseWeapon.Name weaponNameToSpawn)
        :base(name, cost, info, iconName)
    {
        this.weaponNameToSpawn = weaponNameToSpawn;
    }
}

public class WeaponSlotFactory : SlotFactory<WeaponSlotData>
{
    public override ItemSlot Create(Func<ShopBlackboard> ReturnBlackboard)
    {
        ItemSlot slot = Object.Instantiate(_prefab).GetComponent<ItemSlot>();
        slot.Initialize(_data, ReturnBlackboard);
        return slot;
    }
}
