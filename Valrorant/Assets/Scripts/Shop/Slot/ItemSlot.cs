using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Type
    {
        Revive,
        Heal,
        Ammo,
        Weapon
    }

    [SerializeField] TMP_Text _costTxt;

    [SerializeField] Image _iconImg;
    [SerializeField] TMP_Text _nameTxt;
    [SerializeField] protected Button _button;

    string _name;
    string _info;
    Sprite _previewModel;

    protected Func<ShopBlackboard> ReturnBlackboard;

    public virtual void Initialize(WeaponSlotData data, Func<ShopBlackboard> ReturnBlackboard) { }
    public virtual void Initialize(ReviveSlotData data, Func<ShopBlackboard> ReturnBlackboard) { }
    public virtual void Initialize(HealSlotData data, Func<ShopBlackboard> ReturnBlackboard) { }
    public virtual void Initialize(AmmoSlotData data, Func<ShopBlackboard> ReturnBlackboard) { }

    protected virtual void Buy() { }
    protected void ResetSlot(string name, int cost, string info, Database.IconName iconName)
    {
        _name = name;
        _info = info;

        _nameTxt.text = _name;
        _costTxt.text = "$" + cost.ToString();
        _iconImg.sprite = Database.ReturnIcon(iconName);
        _previewModel = Database.ReturnPreview(iconName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ReturnBlackboard().TurnOnPreview?.Invoke(_previewModel, _name, _info);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ReturnBlackboard().TurnOffPreview();
    }
}