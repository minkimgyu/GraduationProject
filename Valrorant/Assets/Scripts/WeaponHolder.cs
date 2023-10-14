using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState
{
    Reload,
    Equip,
    Attack
}

public class WeaponHolder : MonoBehaviour
{
    [SerializeField]
    BaseWeapon[] weapons = new BaseWeapon[3]; // rifle, pistol, knife

    BaseWeapon nowEquipedWeapon = null;
    public BaseWeapon NowEquipedWeapon { get { return nowEquipedWeapon; } }

    int weaponIndex = 0;
    public int WeaponIndex { get { return weaponIndex; } set { weaponIndex = value; } }

    public void Initialize(Transform camera, Animator ownerAnimator)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Initialize(camera, ownerAnimator);
        }
    }

    public void DoUpdate()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].OnUpdate();
        }
    }

    public void ChangeWeapon()
    {
        if (nowEquipedWeapon == weapons[weaponIndex]) return;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == weaponIndex)
            {
                nowEquipedWeapon = weapons[weaponIndex];
                nowEquipedWeapon.OnEquip();
            }
            else
            {
                weapons[i].OnUnEquip();
            }
        }
    }
}
