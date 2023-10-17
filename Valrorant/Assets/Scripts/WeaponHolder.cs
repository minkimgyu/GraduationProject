using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

public enum WeaponState
{
    Reload,
    Equip,
    Attack
}

public class WeaponHolder : MonoBehaviour
{
    IObserver<int, int> _bulletShower;

    [SerializeField]
    BaseWeapon[] weapons = new BaseWeapon[3]; // rifle, pistol, knife

    BaseWeapon nowEquipedWeapon = null;
    public BaseWeapon NowEquipedWeapon { get { return nowEquipedWeapon; } }

    int weaponIndex = 0;
    public int WeaponIndex { get { return weaponIndex; } set { weaponIndex = value; } }

    public void Initialize(GameObject player, Transform camera, Animator ownerAnimator)
    {
        IObserver<int, int> bulletLeftShower = GameObject.FindWithTag("BulletLeftShower").GetComponent<IObserver<int, int>>();
        _bulletShower = bulletLeftShower;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Initialize(player, camera, ownerAnimator);
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

                ISubject<int, int> subject = nowEquipedWeapon.GetComponent<ISubject<int, int>>();
                subject.AddObserver(_bulletShower);

                nowEquipedWeapon.OnEquip();
            }
            else
            {
                ISubject<int, int> subject = weapons[i].GetComponent<ISubject<int, int>>();
                subject.RemoveObserver(_bulletShower);

                weapons[i].OnUnEquip();
            }
        }
    }
}
