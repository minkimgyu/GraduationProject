using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponSlot : ItemSlot
{
    BaseWeapon.Name _weaponNameToSpawn;

    public override void Initialize(WeaponSlotData data, Func<ShopBlackboard> ReturnBlackboard)
    {
        ResetSlot(data.name.ToString(), data.cost, data.info, data.iconName);

        _weaponNameToSpawn = data.weaponNameToSpawn;
        this.ReturnBlackboard = ReturnBlackboard;
        _button.onClick.AddListener(Buy);
    }

    protected override void Buy()
    {
        ShopBlackboard blackboard = ReturnBlackboard();
        BaseWeapon weapon = blackboard.CreateWeapon(_weaponNameToSpawn);
        blackboard.OnBuyWeapon(weapon);
    }
}
