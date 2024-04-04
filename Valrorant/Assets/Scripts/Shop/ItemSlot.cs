using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] GameObject _storedWeapon;

    BaseWeapon _weapon;
    public Action<BaseWeapon> OnSlotClickRequested;

    public void OnClickEventRequested()
    {
        GameObject weaponGo = Instantiate(_storedWeapon);
        BaseWeapon _weapon = weaponGo.GetComponent<BaseWeapon>();

        OnSlotClickRequested?.Invoke(_weapon);
    }
}
