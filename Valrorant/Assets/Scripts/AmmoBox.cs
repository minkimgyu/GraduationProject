using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IEquipedWeapon equipedWeapon = other.GetComponent<IEquipedWeapon>();
        if (equipedWeapon == null) return;

        BaseWeapon weapon = equipedWeapon.ReturnNowEquipedWeapon();
        weapon.RefillAmmo();
        Destroy(gameObject);
    }
}
