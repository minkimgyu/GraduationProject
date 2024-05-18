using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponSlot : ItemSlot
{
    BaseWeapon.Name _weaponNameToSpawn;

    Func<BaseWeapon.Name, BaseWeapon> CreateWeapon;

    public override void ResetData(BaseWeapon.Name name) 
    {
        _weaponNameToSpawn = name;
    }

    public override void Initialize(string name, int cost, string info, Sprite icon, Sprite previewModel, Func<ShopBlackboard> ReturnBlackboard,
    Action TurnOffPreview, Action<Sprite, string, string> TurnOnPreview)
    {
        base.Initialize(name, cost, info, icon, previewModel, ReturnBlackboard, TurnOffPreview, TurnOnPreview);

        WeaponPlant plant = FindObjectOfType<WeaponPlant>();
        CreateWeapon = plant.Create;
    }

    protected override void Buy()
    {
        ShopBlackboard blackboard = ReturnBlackboard();
        BaseWeapon weapon = CreateWeapon(_weaponNameToSpawn);
        blackboard.OnBuyWeapon(weapon);
    }
}
