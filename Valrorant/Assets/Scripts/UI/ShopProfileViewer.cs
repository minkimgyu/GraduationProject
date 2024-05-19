using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class ShopProfileViewer : MonoBehaviour, IDropHandler
{
    [SerializeField] Image _profile;
    [SerializeField] TMP_Text _name;

    [SerializeField] Image _weaponPreviewPrefab;
    [SerializeField] Transform _previewContent;

    Dictionary<BaseWeapon.Type, Image> _weaponPreviewContainer = new Dictionary<BaseWeapon.Type, Image>();

    Action<CharacterPlant.Name> CallShopingEvent;
    CharacterPlant.Name _characterName;
    public void Initialize(Database.PersonName profileName, Action<CharacterPlant.Name> CallShopingEvent)
    {
        _characterName = (CharacterPlant.Name)Enum.Parse(typeof(CharacterPlant.Name), profileName.ToString());

        _name.text = profileName.ToString();
        _profile.sprite = Database.ReturnProfile(profileName);
        this.CallShopingEvent = CallShopingEvent;
    }

    public void AddWeaponPreview(BaseWeapon.Name weaponName, BaseWeapon.Type weaponType)
    {
        Database.IconName name = (Database.IconName)Enum.Parse(typeof(Database.IconName), weaponName.ToString() + "Icon");
        Sprite sprite = Database.ReturnIcon(name);

        Image preview = Instantiate(_weaponPreviewPrefab, _previewContent);
        preview.sprite = sprite;
        preview.transform.SetSiblingIndex((int)weaponType);

        _weaponPreviewContainer[weaponType] = preview;
    }

    public void RemoveWeaponPreview(BaseWeapon.Type weaponType)
    {
        if(_weaponPreviewContainer.ContainsKey(weaponType))
        {
            Destroy(_weaponPreviewContainer[weaponType].gameObject);
        }

        _weaponPreviewContainer.Remove(weaponType);
    }

    public void OnDrop(PointerEventData eventData)
    {
        CallShopingEvent?.Invoke(_characterName);
    }
}
