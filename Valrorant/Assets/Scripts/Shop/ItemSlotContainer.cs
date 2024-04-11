using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Shop
{
    public class ItemSlotContainer : MonoBehaviour
    {
        
        [SerializeField] TMP_Text _titleTxt;
        [SerializeField] ItemSlot _slotPrefab;

        public List<ItemSlot> _itemSlots;
        public void Initialize(string title, List<ItemData> datas) // 여기에 이벤트 추가
        {
            for (int i = 0; i < datas.Count; i++)
            {
                ItemSlot slot = Instantiate(_slotPrefab, transform);
                //slot.Initialize(datas[i]);

                _itemSlots.Add(slot);
            }
        }
    }
}