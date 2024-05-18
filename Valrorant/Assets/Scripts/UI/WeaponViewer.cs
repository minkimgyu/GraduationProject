using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponViewer : MonoBehaviour
{
    [SerializeField] WeaponPreview _previewPrefab;
    [SerializeField] Transform _content;

    Dictionary<BaseWeapon.Type, WeaponPreview> _previewContainer;

    private void Awake()
    {
        Inintialize();
    }

    void Inintialize()
    {
        _previewContainer = new Dictionary<BaseWeapon.Type, WeaponPreview>();

        int size = Enum.GetValues(typeof(BaseWeapon.Type)).Length;
        for (int i = 0; i < size; i++)
        {
            WeaponPreview preview = Instantiate(_previewPrefab, _content);
            preview.TurnOffPreview();

            _previewContainer.Add((BaseWeapon.Type)i, preview);
        }
    }

    public void AddPreview(BaseWeapon.Name weaponName, BaseWeapon.Type weaponType)
    {
        Database.IconName name = (Database.IconName)Enum.Parse(typeof(Database.IconName), weaponName.ToString() + "Icon");
        Sprite sprite = Database.ReturnIcon(name);

        _previewContainer[weaponType].TurnOnPreview(sprite, (int)weaponType + 1);
    }

    public void RemovePreview(BaseWeapon.Type weaponType)
    {
        _previewContainer[weaponType].TurnOffPreview();
    }
}
