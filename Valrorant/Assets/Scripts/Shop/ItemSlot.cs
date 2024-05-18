using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ItemData
{
    [Serializable]
    public enum Name
    {
        Armor,
        AidKit,
        Ammo,

        Glock18,
        MP5,

        AKM,
        M416,
        Scout,

        Saiga,
        MK18,
        M249
    }

    protected Name _name;
    protected int _cost;
    protected string _info;
    protected Sprite _icon;
    protected Sprite _previewModel;

    public ItemData(Name name, int cost, string info, Sprite icon, Sprite previewModel)
    {
        _name = name;
        _cost = cost;

        _info = info;
        _icon = icon;
        _previewModel = previewModel;
    }

    public virtual void Reset(ItemSlot slot, Func<ShopBlackboard> ReturnBlackboard, Action TurnOffPreview, Action<Sprite, string, string> TurnOnPreview) 
    {
        slot.Initialize(_name.ToString(), _cost, _info, _icon, _previewModel, ReturnBlackboard, TurnOffPreview, TurnOnPreview);
    }
}

public class AmmoItemData : ItemData
{
    public AmmoItemData(Name name, int cost, string info, Sprite icon, Sprite previewModel)
        : base(name, cost, info, icon, previewModel)
    {
    }
}

public class HealItemData : ItemData
{
    float _hpPoint;
    float _armorPoint;

    public HealItemData(Name name, float hpPoint, float armorPoint, int cost, string info, Sprite icon, Sprite previewModel)
        : base(name, cost, info, icon, previewModel)
    {
        _hpPoint = hpPoint;
        _armorPoint = armorPoint;
    }

    public override void Reset(ItemSlot slot, Func<ShopBlackboard> ReturnBlackboard, Action TurnOffPreview, Action<Sprite, string, string> TurnOnPreview)
    {
        base.Reset(slot, ReturnBlackboard, TurnOffPreview, TurnOnPreview);
        slot.ResetData(_hpPoint, _armorPoint);
    }
}

public class WeaponItemData : ItemData
{
    BaseWeapon.Name _weaponNameToSpawn;

    public WeaponItemData(Name name, BaseWeapon.Name weaponNameToSpawn, int cost, string info, Sprite icon, Sprite previewModel)
        :base(name, cost, info, icon, previewModel)
    {
        _weaponNameToSpawn = weaponNameToSpawn;
    }

    public override void Reset(ItemSlot slot, Func<ShopBlackboard> ReturnBlackboard, Action TurnOffPreview, Action<Sprite, string, string> TurnOnPreview)
    {
        base.Reset(slot, ReturnBlackboard, TurnOffPreview, TurnOnPreview);
        slot.ResetData(_weaponNameToSpawn);
    }
}

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Type
    {
        Consumable,
        Ammo,
        Weapon
    }

    [SerializeField] TMP_Text _costTxt;

    [SerializeField] Image _iconImg;
    [SerializeField] TMP_Text _nameTxt;
    [SerializeField] Button _button;

    string _name;
    string _info;
    Sprite _previewModel;

    protected Func<ShopBlackboard> ReturnBlackboard;
    Action<Sprite, string, string> TurnOnPreview;
    Action TurnOffPreview;

    public virtual void Initialize(string name, int cost, string info, Sprite icon, Sprite previewModel, Func<ShopBlackboard> ReturnBlackboard, 
        Action TurnOffPreview, Action<Sprite, string, string> TurnOnPreview)
    {
        ResetSlot(name, cost, info, icon, previewModel);
        this.ReturnBlackboard = ReturnBlackboard;

        this.TurnOnPreview = TurnOnPreview;
        this.TurnOffPreview = TurnOffPreview;

        _button.onClick.AddListener(Buy);
    }

    public virtual void ResetData(BaseWeapon.Name name) { }
    public virtual void ResetData(float hpPoint, float armorPoint) { }

    protected virtual void Buy() { }

    void ResetSlot(string name, int cost, string info, Sprite icon, Sprite previewModel)
    {
        _name = name;
        _info = info;

        _nameTxt.text = _name;
        _costTxt.text = "$" + cost.ToString();
        _iconImg.sprite = icon;
        _previewModel = previewModel;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TurnOnPreview(_previewModel, _name, _info);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TurnOffPreview();
    }
}