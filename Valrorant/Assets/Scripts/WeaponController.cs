using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState
{
    Reload,
    Equip,
    Attack
}

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    BaseWeapon[] weapons = new BaseWeapon[3]; // rifle, pistol, knife

    BaseWeapon nowEquipedWeapon = null;
    public BaseWeapon NowEquipedWeapon { get { return nowEquipedWeapon; } }

    public void Initialize(Transform owner, Transform camera, Animator ownerAnimator)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Initialize(owner, camera, ownerAnimator);
        }
    }

    public void ChangeWeapon(int index)
    {
        if (nowEquipedWeapon == weapons[index]) return;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == index)
            {
                nowEquipedWeapon = weapons[index];
                nowEquipedWeapon.OnEquip();
            }
            else
            {
                weapons[i].OnUnEquip();
            }
        }
    }
}
