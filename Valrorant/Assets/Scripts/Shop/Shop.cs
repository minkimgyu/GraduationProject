using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Agent.Controller;

namespace Shop
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] GameObject _slotContainer;

        public Action<BaseWeapon> AddWeapon;
        ItemSlot[] itemSlots;

        private void Start() => Initialized();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (_slotContainer.activeSelf == true) _slotContainer.SetActive(false);
                else _slotContainer.SetActive(true);
            }
        }

        private void Initialized()
        {
            WeaponController weaponController = GameObject.FindWithTag("Player").GetComponent<WeaponController>();
            AddWeapon = weaponController.AddWeapon;

            itemSlots = _slotContainer.GetComponentsInChildren<ItemSlot>();
            for (int i = 0; i < itemSlots.Length; i++)
            {
                //itemSlots[i].OnSlotClickRequested = Buy;
            }
        }

        public void Buy(BaseWeapon baseWeapon)
        {
            AddWeapon(baseWeapon);
        }
    }
}