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
        if (ReturnBlackboard().CanBuy(_cost) == false) return;

        ReturnBlackboard().Buy(_cost);

        ShopBlackboard blackboard = ReturnBlackboard();
        BaseWeapon weapon = blackboard.CreateWeapon(_weaponNameToSpawn);
        blackboard.OnBuyWeapon(weapon);
    }

    protected override void BuyToHelper(CharacterPlant.Name name)
    {
        if (ReturnBlackboard().CanBuy(_cost) == false) return;

        ReturnBlackboard().Buy(_cost);

        ShopBlackboard blackboard = ReturnBlackboard();
        BaseWeapon weapon = blackboard.CreateWeapon(_weaponNameToSpawn);
        blackboard.BuyWeaponToHelper?.Invoke(name, weapon);
    }
}
