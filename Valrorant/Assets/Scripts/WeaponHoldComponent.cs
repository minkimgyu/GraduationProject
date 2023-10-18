using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

public class WeaponHoldComponent : MonoBehaviour
{
    IObserver<int, int> _bulletShower;

    [SerializeField]
    AbstractRoutineContainerClass[] _weaponsContainers = new AbstractRoutineContainerClass[3];
    BaseWeapon[] _weapons = new BaseWeapon[3];

    BaseWeapon _nowEquipedWeapon = null;
    public BaseWeapon NowEquipedWeapon { get { return _nowEquipedWeapon; } }

    int _weaponIndex = 0;
    public int WeaponIndex { get { return _weaponIndex; } set { _weaponIndex = value; } }

    public void Initialize(GameObject player, Transform camera, Animator ownerAnimator)
    {
        IObserver<int, int> bulletLeftShower = GameObject.FindWithTag("BulletLeftShower").GetComponent<IObserver<int, int>>();
        _bulletShower = bulletLeftShower;

        for (int i = 0; i < _weaponsContainers.Length; i++)
        {
            _weapons[i] = (BaseWeapon)_weaponsContainers[i].BaseAbstractRoutineClass;
            _weapons[i].Initialize(player, camera, ownerAnimator);
        }
    }

    public void ChangeWeapon()
    {
        if (_nowEquipedWeapon == _weapons[_weaponIndex]) return;

        for (int i = 0; i < _weapons.Length; i++)
        {
            if (i == _weaponIndex)
            {
                _nowEquipedWeapon = _weapons[_weaponIndex];

                ISubject<int, int> subject = _nowEquipedWeapon;
                subject.AddObserver(_bulletShower);

                _nowEquipedWeapon.OnEquip();
            }
            else
            {
                ISubject<int, int> subject = _weapons[i];
                subject.RemoveObserver(_bulletShower);

                _weapons[i].OnUnEquip();
            }
        }
    }
}
