using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DragSlot : MonoBehaviour
{
    [SerializeField] Image _icon;
    Action<CharacterPlant.Name> BuyItemToHelper;

    public void OnDragStart(Sprite iconSprite, Vector3 pos, Action<CharacterPlant.Name> BuyItemToHelper)
    {
        _icon.enabled = true;
        _icon.sprite = iconSprite;
        transform.position = pos;
        this.BuyItemToHelper = BuyItemToHelper;
    }

    public void OnDraging(Vector3 pos)
    {
        transform.position = pos;
    }

    public void OnDragEnd()
    {
        _icon.enabled = false;
        _icon.sprite = null;
    }

    public void CallShopingEvent(CharacterPlant.Name name)
    {
        BuyItemToHelper?.Invoke(name);
        _icon.enabled = false;
        _icon.sprite = null;
    }
}
