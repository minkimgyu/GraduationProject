using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ItemSlotContainer : MonoBehaviour
{
    [SerializeField] TMP_Text _titleTxt;
    [SerializeField] Transform _content;

    public void Initialize(string title, List<SlotPlant.Name> slotNames, Func<ShopBlackboard> ReturnBlackboard)
    {
        SlotPlant plant = FindObjectOfType<SlotPlant>();
        _titleTxt.text = title;

        for (int i = 0; i < slotNames.Count; i++)
        {
            ItemSlot slot = plant.Create(slotNames[i], ReturnBlackboard);
            slot.transform.SetParent(_content);
        }
    }
}