using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ItemSlotContainer : MonoBehaviour
{
    [SerializeField] TMP_Text _titleTxt;
    [SerializeField] ItemSlotDictionary _itemSlotDictionary;

    [SerializeField] Transform _content;

    public List<ItemSlot> _itemSlots;
    public void Initialize(string title, ItemSlot.Type type, List<ItemData> datas, Func<ShopBlackboard> ReturnBlackboard,
        Action TurnOffPreview, Action<Sprite, string, string> TurnOnPreview)
    {
        _titleTxt.text = title;

        for (int i = 0; i < datas.Count; i++)
        {
            ItemSlot slot = Instantiate(_itemSlotDictionary[type], _content);
            datas[i].Reset(slot, ReturnBlackboard, TurnOffPreview, TurnOnPreview);

            _itemSlots.Add(slot);
        }
    }
}